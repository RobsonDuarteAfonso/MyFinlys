using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Application.Services
{
    public class CardPurchaseService : ICardPurchaseService
    {
        private readonly ICardPurchaseRepository _purchaseRepository;
        private readonly ICardInstallmentRepository _installmentRepository;
        private readonly ICreditCardRepository _cardRepository;
        private readonly IRegisterRepository _registerRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ICardPlanRepository _planRepository;

        public CardPurchaseService(
            ICardPurchaseRepository purchaseRepository,
            ICardInstallmentRepository installmentRepository,
            ICreditCardRepository cardRepository,
            IRegisterRepository registerRepository,
            IBalanceRepository balanceRepository,
            ICardPlanRepository planRepository)
        {
            _purchaseRepository = purchaseRepository;
            _installmentRepository = installmentRepository;
            _cardRepository = cardRepository;
            _registerRepository = registerRepository;
            _balanceRepository = balanceRepository;
            _planRepository = planRepository;
        }

        public async Task<CardPurchaseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _purchaseRepository.GetByIdAsync(id);
            return entity is null ? null : CardPurchaseMapper.ToDto(entity);
        }

        public async Task<IEnumerable<CardPurchaseDto>> GetByCardAsync(Guid cardId)
        {
            var items = await _purchaseRepository.GetByCardAsync(cardId);
            return items.Select(CardPurchaseMapper.ToDto);
        }

        public async Task<IEnumerable<CardInstallmentDto>> GetInstallmentsByCardAndMonthAsync(Guid cardId, string monthName, int year)
        {
            if (!Enum.TryParse<Month>(monthName, true, out var monthEnum))
            {
                throw new ArgumentException("Invalid month name.", nameof(monthName));
            }

            int monthNumber = (int)monthEnum;
            var targetBillingMonth = new DateTime(year, monthNumber, 1);

            var items = await _installmentRepository.GetByCardAndMonthAsync(cardId, targetBillingMonth);
            return items.Select(CardPurchaseMapper.ToInstallmentDto);
        }

        public async Task<Guid> CreateAsync(CardPurchaseCreateDto dto)
        {
            var card = await _cardRepository.GetByIdAsync(dto.CardId);
            if (card is null)
            {
                throw new ArgumentException("Credit Card not found.", nameof(dto.CardId));
            }

            if (!Enum.TryParse<Category>(dto.Category, true, out var category))
            {
                category = Category.Others;
            }

            if (!Enum.TryParse<PurchaseType>(dto.Type, true, out var purchaseType))
            {
                purchaseType = PurchaseType.Credit;
            }

            var totalInstallments = (purchaseType == PurchaseType.Debit || dto.TotalInstallments <= 0) ? 1 : dto.TotalInstallments;

            var purchase = CardPurchase.Create(
                dto.Description,
                dto.PurchaseDate,
                dto.TotalAmount,
                totalInstallments,
                dto.CardId,
                dto.AccountId,
                category,
                purchaseType
            );

            await _purchaseRepository.AddAsync(purchase);

            // Determine the first billing month
            var firstBillingMonth = new DateTime(dto.PurchaseDate.Year, dto.PurchaseDate.Month, 1);
            if (dto.PurchaseDate.Day >= dto.ClosingDay)
            {
                firstBillingMonth = firstBillingMonth.AddMonths(1);
            }

            decimal totalAllocated = 0m;
            decimal baseInstallmentAmount = Math.Round(dto.TotalAmount / totalInstallments, 2);

            var hasLinkedAccount = card.AccountId.HasValue;
            var accountId = card.AccountId.GetValueOrDefault();

            for (int i = 1; i <= totalInstallments; i++)
            {
                decimal amount = baseInstallmentAmount;
                if (i == totalInstallments)
                {
                    amount = dto.TotalAmount - totalAllocated;
                }
                else
                {
                    totalAllocated += baseInstallmentAmount;
                }

                var installmentBillingMonth = firstBillingMonth.AddMonths(i - 1);
                
                // Determine the closing/due day for this installment's billing month
                int targetClosingDay = card.ClosingDay;
                if (installmentBillingMonth.Year == dto.PurchaseDate.Year && installmentBillingMonth.Month == dto.PurchaseDate.Month)
                {
                    targetClosingDay = dto.ClosingDay;
                }
                else
                {
                    // Look if there are existing installments in this target billing month to match their custom closing day
                    var existingInstallmentsInMonth = await _installmentRepository.GetByCardAndMonthAsync(card.Id, installmentBillingMonth);
                    var firstExisting = existingInstallmentsInMonth.FirstOrDefault(x => !x.IsDeleted);
                    if (firstExisting != null)
                    {
                        targetClosingDay = firstExisting.DueDate.Day;
                    }
                }

                var daysInMonth = DateTime.DaysInMonth(installmentBillingMonth.Year, installmentBillingMonth.Month);
                var installmentDueDate = new DateTime(installmentBillingMonth.Year, installmentBillingMonth.Month, Math.Min(targetClosingDay, daysInMonth));

                var installment = CardInstallment.Create(
                    purchase.Id,
                    i,
                    amount,
                    installmentDueDate,
                    Affirmation.No
                );

                purchase.AddInstallment(installment);
                await _installmentRepository.AddAsync(installment);

                if (hasLinkedAccount)
                {
                    var regType = purchaseType == PurchaseType.Debit ? EventType.Credit : EventType.Debit;
                    var targetMonth = (Month)installmentDueDate.Month;
                    var reg = Register.Create(
                        installmentDueDate,
                        regType,
                        i,
                        amount,
                        $"[Cartão] {purchase.Description}",
                        targetMonth,
                        CalculateCalendarWeek(installmentDueDate),
                        Affirmation.No,
                        null,
                        accountId,
                        category
                    );
                    await _registerRepository.AddAsync(reg);
                }
            }

            await _purchaseRepository.SaveChangesAsync();
            await _registerRepository.SaveChangesAsync();

            if (hasLinkedAccount)
            {
                for (int i = 1; i <= totalInstallments; i++)
                {
                    var installmentBillingMonth = firstBillingMonth.AddMonths(i - 1);
                    var targetMonth = (Month)installmentBillingMonth.Month;
                    await RecalculateBalanceAsync(accountId, targetMonth, installmentBillingMonth.Year);
                }
                await _balanceRepository.SaveChangesAsync();
            }

            return purchase.Id;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _purchaseRepository.GetByIdAsync(id);
            if (entity is null)
            {
                return false;
            }

            entity.SoftDelete();
            await _purchaseRepository.UpdateAsync(entity);

            var installments = await _installmentRepository.GetByPurchaseIdAsync(id);
            foreach (var inst in installments)
            {
                inst.SoftDelete();
                await _installmentRepository.UpdateAsync(inst);
            }

            var card = await _cardRepository.GetByIdAsync(entity.CardId);
            if (card != null && card.AccountId.HasValue)
            {
                var accountId = card.AccountId.Value;
                var registers = await _registerRepository.GetByAccountIdAsync(accountId);
                var purchaseRegisters = registers.Where(r => 
                    !r.IsDeleted && 
                    r.Subdescription == $"[Cartão] {entity.Description}"
                ).ToList();
                
                var distinctMonths = purchaseRegisters
                    .Select(r => new { r.Month, Year = r.Due.Year })
                    .Distinct()
                    .ToList();

                foreach (var r in purchaseRegisters)
                {
                    r.SoftDelete();
                    await _registerRepository.UpdateAsync(r);
                }

                foreach (var m in distinctMonths)
                {
                    await RecalculateBalanceAsync(accountId, m.Month, m.Year);
                }
            }

            await _purchaseRepository.SaveChangesAsync();
            await _registerRepository.SaveChangesAsync();
            await _balanceRepository.SaveChangesAsync();
            return true;
        }

        public async Task<CardInstallmentDto?> UpdateInstallmentRealizedAsync(Guid installmentId, string realized)
        {
            var inst = await _installmentRepository.GetByIdAsync(installmentId);
            if (inst is null)
            {
                return null;
            }

            if (!Enum.TryParse<Affirmation>(realized, true, out var realizedEnum))
            {
                throw new ArgumentException("Invalid realization status.", nameof(realized));
            }

            inst.MarkAsRealized(realizedEnum);
            await _installmentRepository.UpdateAsync(inst);

            // Load CardPurchase details for mapping
            var purchase = await _purchaseRepository.GetByIdAsync(inst.CardPurchaseId);
            if (purchase != null)
            {
                // Find matching CardPlan to advance installment if this is a plan installment
                var plans = await _planRepository.GetByCardAsync(purchase.CardId);
                var plan = plans.FirstOrDefault(p =>
                    !p.IsDeleted &&
                    p.Description == purchase.Description &&
                    p.TotalInstallments == purchase.TotalInstallments &&
                    p.OriginalAmount == purchase.TotalAmount);

                if (plan != null && realizedEnum == Affirmation.Yes && inst.InstallmentNumber >= plan.CurrentInstallment)
                {
                    plan.AdvanceInstallment();
                    await _planRepository.UpdateAsync(plan);
                    await _planRepository.SaveChangesAsync();
                }

                var card = await _cardRepository.GetByIdAsync(purchase.CardId);
                if (card != null && card.AccountId.HasValue)
                {
                    var accountId = card.AccountId.Value;
                    var month = (Month)inst.DueDate.Month;
                    var year = inst.DueDate.Year;
                    
                    var registers = await _registerRepository.GetByAccountAndMonthAsync(accountId, month, year);
                    var matchingReg = registers.FirstOrDefault(r => 
                        !r.IsDeleted && 
                        r.Subdescription == $"[Cartão] {purchase.Description}" && 
                        r.InstallmentCurrent == inst.InstallmentNumber && 
                        r.Value == inst.Amount
                    );

                    if (matchingReg != null)
                    {
                        var recalcWeek = Math.Clamp(CalculateCalendarWeek(matchingReg.Due), 1, 6);
                        matchingReg.Update(
                            matchingReg.Due,
                            matchingReg.EventType,
                            matchingReg.InstallmentCurrent,
                            matchingReg.Value,
                            matchingReg.Subdescription,
                            matchingReg.Month,
                            recalcWeek,
                            realizedEnum,
                            matchingReg.EventId,
                            matchingReg.AccountId,
                            matchingReg.Category
                        );
                        await _registerRepository.UpdateAsync(matchingReg);
                        await RecalculateBalanceAsync(accountId, month, year);
                    }
                }
            }

            await _installmentRepository.SaveChangesAsync();
            await _registerRepository.SaveChangesAsync();
            await _balanceRepository.SaveChangesAsync();

            return CardPurchaseMapper.ToInstallmentDto(inst);
        }

        private int CalculateCalendarWeek(DateTime date)
        {
            var firstOfMonth = new DateTime(date.Year, date.Month, 1);
            int firstDayOfWeek = (int)firstOfMonth.DayOfWeek;
            var result = (date.Day + firstDayOfWeek - 1) / 7 + 1;
            return Math.Clamp(result, 1, 6);
        }

        private async Task RecalculateBalanceAsync(Guid accountId, Month month, int year)
        {
            var registers = await _registerRepository.GetByAccountAndMonthAsync(accountId, month, year);
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
        }

        public async Task AdjustClosingDayAsync(Guid cardId, string monthName, int year, int closingDay)
        {
            var card = await _cardRepository.GetByIdAsync(cardId);
            if (card == null) return;

            if (!Enum.TryParse<Month>(monthName, true, out var targetMonthEnum)) return;
            int targetMonthNumber = (int)targetMonthEnum;

            var purchases = await _purchaseRepository.GetByCardAsync(cardId);
            var hasLinkedAccount = card.AccountId.HasValue;
            var accountId = card.AccountId.GetValueOrDefault();

            var monthsToRecalculate = new HashSet<(Month Month, int Year)>();

            foreach (var purchase in purchases)
            {
                if (purchase.IsDeleted) continue;

                var installments = (await _installmentRepository.GetByPurchaseIdAsync(purchase.Id))
                    .Where(i => !i.IsDeleted)
                    .OrderBy(i => i.InstallmentNumber)
                    .ToList();

                if (!installments.Any()) continue;

                foreach (var inst in installments)
                {
                    var oldDueDate = inst.DueDate;

                    // Only update installments due in the target month/year
                    if (oldDueDate.Month != targetMonthNumber || oldDueDate.Year != year)
                        continue;

                    var oldMonth = (Month)oldDueDate.Month;
                    var oldYear = oldDueDate.Year;

                    // Recalculate due date using the new closing day (same month)
                    var daysInMonth = DateTime.DaysInMonth(year, targetMonthNumber);
                    var newDueDate = new DateTime(year, targetMonthNumber, Math.Min(closingDay, daysInMonth));

                    if (inst.DueDate == newDueDate) continue;

                    inst.UpdateDueDate(newDueDate);
                    await _installmentRepository.UpdateAsync(inst);

                    // Sync with bank account register if linked
                    if (hasLinkedAccount)
                    {
                        var registers = await _registerRepository.GetByAccountAndMonthAsync(accountId, oldMonth, oldYear);
                        var matchingReg = registers.FirstOrDefault(r =>
                            !r.IsDeleted &&
                            r.Subdescription == $"[Cartão] {purchase.Description}" &&
                            r.InstallmentCurrent == inst.InstallmentNumber &&
                            r.Value == inst.Amount
                        );

                        if (matchingReg != null)
                        {
                            var newMonth = (Month)newDueDate.Month;
                            var calcWeek = Math.Clamp(CalculateCalendarWeek(newDueDate), 1, 6);
                            Console.WriteLine($"[DEBUG] Updating Register ID {matchingReg.Id}: oldDue={oldDueDate:yyyy-MM-dd}, newDue={newDueDate:yyyy-MM-dd}, week={calcWeek}");
                            matchingReg.Update(
                                newDueDate,
                                matchingReg.EventType,
                                matchingReg.InstallmentCurrent,
                                matchingReg.Value,
                                matchingReg.Subdescription,
                                newMonth,
                                calcWeek,
                                matchingReg.Realized,
                                matchingReg.EventId,
                                matchingReg.AccountId,
                                matchingReg.Category
                            );
                            await _registerRepository.UpdateAsync(matchingReg);

                            monthsToRecalculate.Add((oldMonth, oldYear));
                            monthsToRecalculate.Add((newMonth, newDueDate.Year));
                        }
                    }
                }
            }

            await _purchaseRepository.SaveChangesAsync();
            await _installmentRepository.SaveChangesAsync();
            if (hasLinkedAccount)
            {
                foreach (var m in monthsToRecalculate)
                {
                    await RecalculateBalanceAsync(accountId, m.Month, m.Year);
                }
                await _registerRepository.SaveChangesAsync();
                await _balanceRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CardPurchasePlanDto>> GetPlansByCardAsync(Guid cardId)
        {
            var purchases = await _purchaseRepository.GetByCardAsync(cardId);

            var plans = new List<CardPurchasePlanDto>();

            foreach (var purchase in purchases.Where(p => !p.IsDeleted && p.TotalInstallments > 1))
            {
                var installments = (await _installmentRepository.GetByPurchaseIdAsync(purchase.Id))
                    .Where(i => !i.IsDeleted)
                    .ToList();

                int totalInstallments = purchase.TotalInstallments;
                int paidInstallments = installments.Count(i => i.Realized == Affirmation.Yes);
                decimal remainingBalance = installments
                    .Where(i => i.Realized == Affirmation.No)
                    .Sum(i => i.Amount);

                string status = remainingBalance <= 0m ? "Closed" : "Active";

                decimal monthlyPayment = totalInstallments > 0
                    ? Math.Round(purchase.TotalAmount / totalInstallments, 2)
                    : purchase.TotalAmount;

                plans.Add(new CardPurchasePlanDto
                {
                    Id = purchase.Id,
                    Description = purchase.Description,
                    StartDate = purchase.PurchaseDate,
                    TotalAmount = purchase.TotalAmount,
                    MonthlyPayment = monthlyPayment,
                    RemainingBalance = remainingBalance,
                    Status = status,
                    TotalInstallments = totalInstallments,
                    PaidInstallments = paidInstallments,
                    Category = purchase.Category.ToString(),
                    Type = purchase.Type.ToString()
                });
            }

            return plans.OrderByDescending(p => p.StartDate);
        }

        public async Task<bool> TransferInstallmentToNextMonthAsync(Guid installmentId)
        {
            var inst = await _installmentRepository.GetByIdAsync(installmentId);
            if (inst is null)
            {
                return false;
            }

            var purchase = await _purchaseRepository.GetByIdAsync(inst.CardPurchaseId);
            if (purchase is null)
            {
                return false;
            }

            var card = await _cardRepository.GetByIdAsync(purchase.CardId);
            var oldDueDate = inst.DueDate;
            var newDueDate = oldDueDate.AddMonths(1);

            inst.UpdateDueDate(newDueDate);
            await _installmentRepository.UpdateAsync(inst);

            bool hasLinkedAccount = card != null && card.AccountId.HasValue;
            if (hasLinkedAccount)
            {
                var accountId = card.AccountId.Value;
                var oldMonth = (Month)oldDueDate.Month;
                var oldYear = oldDueDate.Year;
                var registers = await _registerRepository.GetByAccountAndMonthAsync(accountId, oldMonth, oldYear);
                var matchingReg = registers.FirstOrDefault(r =>
                    !r.IsDeleted &&
                    r.Subdescription == $"[Cartão] {purchase.Description}" &&
                    r.InstallmentCurrent == inst.InstallmentNumber &&
                    r.Value == inst.Amount
                );

                if (matchingReg != null)
                {
                    var newMonth = (Month)newDueDate.Month;
                    var calcWeek = Math.Clamp(CalculateCalendarWeek(newDueDate), 1, 6);
                    matchingReg.Update(
                        newDueDate,
                        matchingReg.EventType,
                        matchingReg.InstallmentCurrent,
                        matchingReg.Value,
                        matchingReg.Subdescription,
                        newMonth,
                        calcWeek,
                        matchingReg.Realized,
                        matchingReg.EventId,
                        matchingReg.AccountId,
                        matchingReg.Category
                    );
                    await _registerRepository.UpdateAsync(matchingReg);

                    await RecalculateBalanceAsync(accountId, oldMonth, oldYear);
                    await RecalculateBalanceAsync(accountId, newMonth, newDueDate.Year);
                }
            }

            await _installmentRepository.SaveChangesAsync();
            if (hasLinkedAccount)
            {
                await _registerRepository.SaveChangesAsync();
                await _balanceRepository.SaveChangesAsync();
            }

            return true;
        }
    }
}
