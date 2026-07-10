using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Application.Services;

public class RegisterService : IRegisterService
{
    private readonly IRegisterRepository _repository;
    private readonly IEventRepository _eventRepository;
    private readonly IBalanceRepository _balanceRepository;
    private readonly ICardInstallmentRepository _cardInstallmentRepository;

    public RegisterService(
        IRegisterRepository repository,
        IEventRepository eventRepository,
        IBalanceRepository balanceRepository,
        ICardInstallmentRepository cardInstallmentRepository)
    {
        _repository = repository;
        _eventRepository = eventRepository;
        _balanceRepository = balanceRepository;
        _cardInstallmentRepository = cardInstallmentRepository;
    }

    private async Task EnsureMonthNotClosedAsync(Guid accountId, Month month, int year)
    {
        var yearVO = MyFinlys.Domain.ValueObjects.Year.Create(year);
        var balance = await _balanceRepository.GetByAccountMonthYearAsync(accountId, month, yearVO);
        if (balance != null && balance.IsClosed)
        {
            throw new InvalidOperationException("Cannot modify registers in a closed month.");
        }
    }

    public async Task<RegisterDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : RegisterMapper.ToDto(entity);
    }

    public async Task<IEnumerable<RegisterDto>> GetByEventIdAsync(Guid eventId)
    {
        var items = await _repository.GetByEventIdAsync(eventId);
        return items.Select(RegisterMapper.ToDto);
    }

    public async Task<IEnumerable<RegisterDto>> GetByAccountIdAsync(Guid accountId)
    {
        var items = await _repository.GetByAccountIdAsync(accountId);
        return items.Select(RegisterMapper.ToDto);
    }

    public async Task<Guid> CreateAsync(RegisterCreateDto dto)
    {
        var targetMonth = Enum.Parse<Month>(dto.Month);
        await EnsureMonthNotClosedAsync(dto.AccountId, targetMonth, dto.Due.Year);

        // Normalize the date to UTC noon to prevent timezone-based day shifts
        var normalizedDue = DateTime.SpecifyKind(dto.Due.Date, DateTimeKind.Utc);

        var entity = Register.Create(
            normalizedDue,
            Enum.Parse<EventType>(dto.EventType),
            dto.InstallmentCurrent,
            dto.Value,
            dto.Subdescription,
            targetMonth,
            dto.Week,
            Enum.Parse<Affirmation>(dto.Realized),
            dto.EventId,
            dto.AccountId,
            Enum.Parse<Category>(dto.Category)
        );

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        await RecalculateBalanceAsync(entity.AccountId, entity.Month, entity.Due.Year);

        return entity.Id;
    }

    public async Task<RegisterDto?> UpdateAsync(Guid id, RegisterUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return null;

        await EnsureMonthNotClosedAsync(entity.AccountId, entity.Month, entity.Due.Year);

        var targetMonth = Enum.Parse<Month>(dto.Month);
        if (dto.AccountId != entity.AccountId || targetMonth != entity.Month || dto.Due.Year != entity.Due.Year)
        {
            await EnsureMonthNotClosedAsync(dto.AccountId, targetMonth, dto.Due.Year);
        }

        var oldAccountId = entity.AccountId;
        var oldMonth = entity.Month;
        var oldYear = entity.Due.Year;

        // Normalize the date to UTC noon to prevent timezone-based day shifts
        var normalizedDue = DateTime.SpecifyKind(dto.Due.Date, DateTimeKind.Utc);

        entity.Update(
            normalizedDue,
            Enum.Parse<EventType>(dto.EventType),
            dto.InstallmentCurrent,
            dto.Value,
            dto.Subdescription,
            targetMonth,
            dto.Week,
            Enum.Parse<Affirmation>(dto.Realized),
            dto.EventId,
            dto.AccountId,
            Enum.Parse<Category>(dto.Category)
        );

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();

        await RecalculateBalanceAsync(oldAccountId, oldMonth, oldYear);

        if (entity.AccountId != oldAccountId || entity.Month != oldMonth || entity.Due.Year != oldYear)
        {
            await RecalculateBalanceAsync(entity.AccountId, entity.Month, entity.Due.Year);
        }

        return RegisterMapper.ToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return false;

        await EnsureMonthNotClosedAsync(entity.AccountId, entity.Month, entity.Due.Year);

        var accountId = entity.AccountId;
        var month = entity.Month;
        var year = entity.Due.Year;

        await _repository.PhysicalDeleteAsync(entity.Id);
        await _repository.SaveChangesAsync();

        await RecalculateBalanceAsync(accountId, month, year);

        return true;
    }

    public async Task<IEnumerable<RegisterDto>> GetByAccountAndMonthAsync(Guid accountId, Month month, int year)
    {
        var items = await _repository.GetByAccountAndMonthAsync(accountId, month, year);
        return items.Select(RegisterMapper.ToDto);
    }

    public async Task InitializeMonthsAsync(Guid accountId)
    {
        var today = DateTime.Today;
        var startMonthDate = new DateTime(today.Year, today.Month, 1);
        var allRegisters = (await _repository.GetByAccountIdAsync(accountId)).ToList();

        for (int i = -2; i <= 12; i++)
        {
            var targetDate = startMonthDate.AddMonths(i);
            int targetYear = targetDate.Year;
            var targetMonth = (Month)targetDate.Month;

            var yearVO = MyFinlys.Domain.ValueObjects.Year.Create(targetYear);
            var balance = await _balanceRepository.GetByAccountMonthYearAsync(accountId, targetMonth, yearVO);

            // Get existing registers for this account in the target month and year
            var existingRegisters = allRegisters
                .Where(r => r.Month == targetMonth && r.Due.Year == targetYear && !r.IsDeleted)
                .ToList();
            var existingEventIds = (await _repository.GetEventIdsForMonthIncludingDeletedAsync(accountId, targetMonth, targetYear)).ToHashSet();

            var allEvents = await _eventRepository.GetByAccountIdAsync(accountId);
            var activeEvents = allEvents.Where(e => e.Finished == Affirmation.No);

            bool addedAny = false;
            decimal balanceDelta = 0m;

            foreach (var ev in activeEvents)
            {
                // Only generate occurrences for events that do not have registers generated yet in this month
                // AND (either the month was never initialized OR the event is newer than the initialization of the month)
                if (!existingEventIds.Contains(ev.Id) && (balance == null || ev.CreatedAt > balance.CreatedAt))
                {
                    var registersToCreate = GenerateEventOccurrences(ev, targetYear, targetDate.Month, accountId, allRegisters);
                    foreach (var reg in registersToCreate)
                    {
                        await _repository.AddAsync(reg);
                        allRegisters.Add(reg);
                        addedAny = true;
                        if (reg.EventType == EventType.Credit)
                        {
                            balanceDelta += reg.Value;
                        }
                        else if (reg.EventType == EventType.Debit)
                        {
                            balanceDelta -= reg.Value;
                        }
                    }
                }
            }

            // Auto apply Card Installments linked to this account
            var cardInstallments = await _cardInstallmentRepository.GetByAccountAndMonthAsync(accountId, targetDate);
            foreach (var ci in cardInstallments)
            {
                bool exists = existingRegisters.Any(r => 
                    r.Subdescription == $"[Cartão] {ci.CardPurchase.Description}" && 
                    r.InstallmentCurrent == ci.InstallmentNumber && 
                    r.Value == ci.Amount);

                if (!exists)
                {
                    var regType = ci.CardPurchase.Type == PurchaseType.Debit ? EventType.Credit : EventType.Debit;
                    var reg = Register.Create(
                        ci.DueDate,
                        regType,
                        ci.InstallmentNumber,
                        ci.Amount,
                        $"[Cartão] {ci.CardPurchase.Description}",
                        targetMonth,
                        CalculateCalendarWeek(ci.DueDate),
                        ci.Realized,
                        null,
                        accountId,
                        ci.CardPurchase.Category
                    );

                    await _repository.AddAsync(reg);
                    allRegisters.Add(reg);
                    addedAny = true;
                    if (reg.EventType == EventType.Credit)
                    {
                        balanceDelta += reg.Value;
                    }
                    else if (reg.EventType == EventType.Debit)
                    {
                        balanceDelta -= reg.Value;
                    }
                }
            }

            var balanceYearVO = MyFinlys.Domain.ValueObjects.Year.Create(targetYear);
            var existingBalance = await _balanceRepository.GetByAccountMonthYearAsync(accountId, targetMonth, balanceYearVO);

            if (existingBalance != null)
            {
                if (addedAny)
                {
                    existingBalance.UpdateAmount(existingBalance.Amount + balanceDelta);
                    
                    bool hasPassed = (targetYear < today.Year) || (targetYear == today.Year && targetDate.Month < today.Month);
                    if (hasPassed)
                    {
                        var updatedRegisters = allRegisters.Where(r => r.Month == targetMonth && r.Due.Year == targetYear && !r.IsDeleted);
                        bool hasUnrealized = updatedRegisters.Any(r => r.Realized == Affirmation.No);
                        if (!hasUnrealized) existingBalance.Close();
                        else existingBalance.Open();
                    }

                    await _balanceRepository.UpdateAsync(existingBalance);
                }
            }
            else
            {
                // If no balance exists, calculate total balance based on all registers (both old and newly added ones)
                decimal totalBalance = 0m;
                var updatedRegisters = allRegisters.Where(r => r.Month == targetMonth && r.Due.Year == targetYear && !r.IsDeleted).ToList();
                foreach (var reg in updatedRegisters)
                {
                    if (reg.EventType == EventType.Credit)
                        totalBalance += reg.Value;
                    else if (reg.EventType == EventType.Debit)
                        totalBalance -= reg.Value;
                }
                
                var newBalance = Balance.Create(accountId, targetYear, targetMonth, totalBalance, false);
                await _balanceRepository.AddAsync(newBalance);
            }
        }

        await _repository.SaveChangesAsync();
        await _balanceRepository.SaveChangesAsync();
    }

    private async Task RecalculateBalanceAsync(Guid accountId, Month month, int year)
    {
        var registers = await _repository.GetByAccountAndMonthAsync(accountId, month, year);
        decimal balanceAmount = 0m;
        foreach (var reg in registers)
        {
            if (reg.IsDeleted) continue;
            if (reg.EventType == EventType.Credit)
            {
                balanceAmount += reg.Value;
            }
            else if (reg.EventType == EventType.Debit)
            {
                balanceAmount -= reg.Value;
            }
        }

        var yearVO = MyFinlys.Domain.ValueObjects.Year.Create(year);
        var balance = await _balanceRepository.GetByAccountMonthYearAsync(accountId, month, yearVO);
        if (balance != null)
        {
            balance.UpdateAmount(balanceAmount);
            await _balanceRepository.UpdateAsync(balance);
        }
        else
        {
            balance = Balance.Create(accountId, year, month, balanceAmount, false);
            await _balanceRepository.AddAsync(balance);
        }
        await _balanceRepository.SaveChangesAsync();
    }

    public async Task CloseMonthAsync(Guid accountId, Month month, int year)
    {
        var registers = (await _repository.GetByAccountAndMonthAsync(accountId, month, year)).ToList();

        bool anyChanged = false;
        foreach (var reg in registers)
        {
            if (reg.Realized == Affirmation.No)
            {
                reg.MarkRealized();
                await _repository.UpdateAsync(reg);
                anyChanged = true;
            }
        }

        if (anyChanged)
        {
            await _repository.SaveChangesAsync();
        }

        decimal balanceAmount = 0m;
        foreach (var reg in registers)
        {
            if (reg.IsDeleted) continue;
            if (reg.EventType == EventType.Credit)
            {
                balanceAmount += reg.Value;
            }
            else if (reg.EventType == EventType.Debit)
            {
                balanceAmount -= reg.Value;
            }
        }

        var yearVO = MyFinlys.Domain.ValueObjects.Year.Create(year);
        var balance = await _balanceRepository.GetByAccountMonthYearAsync(accountId, month, yearVO);
        if (balance != null)
        {
            balance.UpdateAmount(balanceAmount);
            balance.Close();
            await _balanceRepository.UpdateAsync(balance);
        }
        else
        {
            balance = Balance.Create(accountId, year, month, balanceAmount, true);
            await _balanceRepository.AddAsync(balance);
        }
        await _balanceRepository.SaveChangesAsync();
    }


    private int CalculateCalendarWeek(DateTime date)
    {
        var firstOfMonth = new DateTime(date.Year, date.Month, 1);
        int firstDayOfWeek = (int)firstOfMonth.DayOfWeek;
        return (date.Day + firstDayOfWeek - 1) / 7 + 1;
    }

    private int GetCorrectInstallment(Event ev, DateTime due, List<Register> allRegisters)
    {
        if (ev.Installment == null)
            return 1;

        // Find the latest register for this event that has a due date before the current one
        var latestPastRegister = allRegisters
            .Where(r => r.EventId == ev.Id && r.Due.Date < due.Date && !r.IsDeleted)
            .OrderByDescending(r => r.Due)
            .FirstOrDefault();

        if (latestPastRegister != null)
        {
            if (ev.Period == EventPeriod.Weekly)
            {
                int weeksDiff = (int)((due.Date - latestPastRegister.Due.Date).TotalDays / 7);
                return latestPastRegister.InstallmentCurrent + weeksDiff;
            }
            else if (ev.Period == EventPeriod.Biweekly)
            {
                int biweeksDiff = (int)((due.Date - latestPastRegister.Due.Date).TotalDays / 14);
                return latestPastRegister.InstallmentCurrent + biweeksDiff;
            }
            else if (ev.Period == EventPeriod.Monthly)
            {
                int monthsDiff = ((due.Year - latestPastRegister.Due.Year) * 12) + (due.Month - latestPastRegister.Due.Month);
                return latestPastRegister.InstallmentCurrent + monthsDiff;
            }
            else if (ev.Period == EventPeriod.Quarterly)
            {
                int monthsDiff = ((due.Year - latestPastRegister.Due.Year) * 12) + (due.Month - latestPastRegister.Due.Month);
                return latestPastRegister.InstallmentCurrent + (monthsDiff / 3);
            }
            else if (ev.Period == EventPeriod.SemiAnnual)
            {
                int monthsDiff = ((due.Year - latestPastRegister.Due.Year) * 12) + (due.Month - latestPastRegister.Due.Month);
                return latestPastRegister.InstallmentCurrent + (monthsDiff / 6);
            }
            else if (ev.Period == EventPeriod.Annual)
            {
                int monthsDiff = ((due.Year - latestPastRegister.Due.Year) * 12) + (due.Month - latestPastRegister.Due.Month);
                return latestPastRegister.InstallmentCurrent + (monthsDiff / 12);
            }
        }

        // Fallback to base calculations from event start/initial date
        if (ev.Period == EventPeriod.Weekly && ev is EventWeekly evWeekly)
        {
            if (evWeekly.Installment != null && evWeekly.Installment.DateInitial.HasValue)
            {
                int weeksDiff = (int)((due.Date - evWeekly.Installment.DateInitial.Value.Date).TotalDays / 7);
                return 1 + weeksDiff;
            }
        }
        else if (ev.Period == EventPeriod.Biweekly && ev is EventBiweekly evBiweekly)
        {
            if (evBiweekly.Installment != null)
            {
                var baseDate = evBiweekly.Installment.DateInitial ?? evBiweekly.StartDate;
                int biweeksDiff = (int)((due.Date - baseDate.Date).TotalDays / 14);
                return 1 + biweeksDiff;
            }
        }
        else if (ev.Period == EventPeriod.Monthly && ev is EventMonthly evMonthly)
        {
            if (evMonthly.Installment != null)
            {
                int monthsDiff = ((due.Year - evMonthly.Due.Year) * 12) + (due.Month - evMonthly.Due.Month);
                return 1 + monthsDiff;
            }
        }
        else if (ev.Period == EventPeriod.Quarterly && ev is EventQuarterly evQuarterly)
        {
            if (evQuarterly.Installment != null)
            {
                int monthsDiff = ((due.Year - evQuarterly.Due.Year) * 12) + (due.Month - evQuarterly.Due.Month);
                return 1 + (monthsDiff / 3);
            }
        }
        else if (ev.Period == EventPeriod.SemiAnnual && ev is EventSemiAnnual evSemi)
        {
            if (evSemi.Installment != null)
            {
                int monthsDiff = ((due.Year - evSemi.Due.Year) * 12) + (due.Month - evSemi.Due.Month);
                return 1 + (monthsDiff / 6);
            }
        }
        else if (ev.Period == EventPeriod.Annual && ev is EventAnnual evAnnual)
        {
            if (evAnnual.Installment != null)
            {
                int monthsDiff = ((due.Year - evAnnual.Due.Year) * 12) + (due.Month - evAnnual.Due.Month);
                return 1 + (monthsDiff / 12);
            }
        }

        return 1;
    }

    private List<Register> GenerateEventOccurrences(Event ev, int targetYear, int targetMonthNumber, Guid accountId, List<Register> allRegisters)
    {
        var list = new List<Register>();
        var targetMonth = (Month)targetMonthNumber;

        var startOfTargetMonth = new DateTime(targetYear, targetMonthNumber, 1);
        if (ev.EndDate.HasValue && ev.EndDate.Value.Date < startOfTargetMonth)
        {
            return list;
        }

        switch (ev.Period)
        {
            case EventPeriod.Weekly:
                if (ev is EventWeekly evWeekly)
                {
                    int daysInMonth = DateTime.DaysInMonth(targetYear, targetMonthNumber);
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        var due = new DateTime(targetYear, targetMonthNumber, day);
                        if (due.DayOfWeek == evWeekly.DayOfWeek)
                        {
                            if (evWeekly.EndDate.HasValue && due.Date > evWeekly.EndDate.Value.Date)
                                continue;

                            int installmentCurrent = GetCorrectInstallment(evWeekly, due, allRegisters);
                            if (evWeekly.Installment != null && (installmentCurrent > evWeekly.Installment.InstallmentTotal || installmentCurrent < 1))
                                continue;

                            int week = CalculateCalendarWeek(due);
                            var reg = Register.Create(
                                due,
                                evWeekly.Type,
                                installmentCurrent,
                                evWeekly.Value,
                                evWeekly.Description,
                                targetMonth,
                                week,
                                Affirmation.No,
                                evWeekly.Id,
                                accountId,
                                evWeekly.Category
                            );
                            list.Add(reg);
                        }
                    }
                }
                break;

            case EventPeriod.Biweekly:
                if (ev is EventBiweekly evBiweekly)
                {
                    var current = evBiweekly.StartDate;
                    // Align the starting date to the configured DayOfWeek
                    while (current.DayOfWeek != evBiweekly.DayOfWeek)
                    {
                        current = current.AddDays(1);
                    }
                    var maxDate = DateTime.Today.AddYears(2);
                    while (current <= maxDate)
                    {
                        if (evBiweekly.EndDate.HasValue && current.Date > evBiweekly.EndDate.Value.Date)
                            break;

                        if (current.Year == targetYear && current.Month == targetMonthNumber)
                        {
                            int installmentCurrent = GetCorrectInstallment(evBiweekly, current, allRegisters);
                            if (evBiweekly.Installment != null && (installmentCurrent > evBiweekly.Installment.InstallmentTotal || installmentCurrent < 1))
                            {
                                current = current.AddDays(14);
                                continue;
                            }

                            int week = CalculateCalendarWeek(current);
                            var reg = Register.Create(
                                current,
                                evBiweekly.Type,
                                installmentCurrent,
                                evBiweekly.Value,
                                evBiweekly.Description,
                                targetMonth,
                                week,
                                Affirmation.No,
                                evBiweekly.Id,
                                accountId,
                                evBiweekly.Category
                            );
                            list.Add(reg);
                        }
                        current = current.AddDays(14);
                    }
                }
                break;

            case EventPeriod.Monthly:
                if (ev is EventMonthly evMonthly)
                {
                    int day = Math.Min(evMonthly.Due.Day, DateTime.DaysInMonth(targetYear, targetMonthNumber));
                    var due = new DateTime(targetYear, targetMonthNumber, day);
                    if (due.Date >= evMonthly.Due.Date && (!evMonthly.EndDate.HasValue || due.Date <= evMonthly.EndDate.Value.Date))
                    {
                        int installmentCurrent = GetCorrectInstallment(evMonthly, due, allRegisters);
                        if (evMonthly.Installment != null && installmentCurrent > evMonthly.Installment.InstallmentTotal)
                            break;

                        int week = CalculateCalendarWeek(due);
                        var reg = Register.Create(
                            due,
                            evMonthly.Type,
                            installmentCurrent,
                            evMonthly.Value,
                            evMonthly.Description,
                            targetMonth,
                            week,
                            Affirmation.No,
                            evMonthly.Id,
                            accountId,
                            evMonthly.Category
                        );
                        list.Add(reg);
                    }
                }
                break;

            case EventPeriod.Quarterly:
                if (ev is EventQuarterly evQuarterly)
                {
                    int monthsDiff = ((targetYear - evQuarterly.Due.Year) * 12) + (targetMonthNumber - evQuarterly.Due.Month);
                    if (monthsDiff >= 0 && monthsDiff % 3 == 0)
                    {
                        int day = Math.Min(evQuarterly.Due.Day, DateTime.DaysInMonth(targetYear, targetMonthNumber));
                        var due = new DateTime(targetYear, targetMonthNumber, day);
                        if (!evQuarterly.EndDate.HasValue || due.Date <= evQuarterly.EndDate.Value.Date)
                        {
                            int installmentCurrent = GetCorrectInstallment(evQuarterly, due, allRegisters);
                            if (evQuarterly.Installment != null && installmentCurrent > evQuarterly.Installment.InstallmentTotal)
                                break;

                            int week = CalculateCalendarWeek(due);
                            var reg = Register.Create(
                                due,
                                evQuarterly.Type,
                                installmentCurrent,
                                evQuarterly.Value,
                                evQuarterly.Description,
                                targetMonth,
                                week,
                                Affirmation.No,
                                evQuarterly.Id,
                                accountId,
                                evQuarterly.Category
                            );
                            list.Add(reg);
                        }
                    }
                }
                break;

            case EventPeriod.SemiAnnual:
                if (ev is EventSemiAnnual evSemi)
                {
                    int monthsDiff = ((targetYear - evSemi.Due.Year) * 12) + (targetMonthNumber - evSemi.Due.Month);
                    if (monthsDiff >= 0 && monthsDiff % 6 == 0)
                    {
                        int day = Math.Min(evSemi.Due.Day, DateTime.DaysInMonth(targetYear, targetMonthNumber));
                        var due = new DateTime(targetYear, targetMonthNumber, day);
                        if (!evSemi.EndDate.HasValue || due.Date <= evSemi.EndDate.Value.Date)
                        {
                            int installmentCurrent = GetCorrectInstallment(evSemi, due, allRegisters);
                            if (evSemi.Installment != null && installmentCurrent > evSemi.Installment.InstallmentTotal)
                                break;

                            int week = CalculateCalendarWeek(due);
                            var reg = Register.Create(
                                due,
                                evSemi.Type,
                                installmentCurrent,
                                evSemi.Value,
                                evSemi.Description,
                                targetMonth,
                                week,
                                Affirmation.No,
                                evSemi.Id,
                                accountId,
                                evSemi.Category
                            );
                            list.Add(reg);
                        }
                    }
                }
                break;

            case EventPeriod.Annual:
                if (ev is EventAnnual evAnnual)
                {
                    int monthsDiff = ((targetYear - evAnnual.Due.Year) * 12) + (targetMonthNumber - evAnnual.Due.Month);
                    if (monthsDiff >= 0 && monthsDiff % 12 == 0)
                    {
                        int day = Math.Min(evAnnual.Due.Day, DateTime.DaysInMonth(targetYear, targetMonthNumber));
                        var due = new DateTime(targetYear, targetMonthNumber, day);
                        if (!evAnnual.EndDate.HasValue || due.Date <= evAnnual.EndDate.Value.Date)
                        {
                            int installmentCurrent = GetCorrectInstallment(evAnnual, due, allRegisters);
                            if (evAnnual.Installment != null && installmentCurrent > evAnnual.Installment.InstallmentTotal)
                                break;

                            int week = CalculateCalendarWeek(due);
                            var reg = Register.Create(
                                due,
                                evAnnual.Type,
                                installmentCurrent,
                                evAnnual.Value,
                                evAnnual.Description,
                                targetMonth,
                                week,
                                Affirmation.No,
                                evAnnual.Id,
                                accountId,
                                evAnnual.Category
                            );
                            list.Add(reg);
                        }
                    }
                }
                break;
        }

        return list;
    }
}

