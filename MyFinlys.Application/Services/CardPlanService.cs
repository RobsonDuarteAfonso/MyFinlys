using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Services.Mappers;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Application.Services
{
    public class CardPlanService : ICardPlanService
    {
        private readonly ICardPlanRepository _planRepository;
        private readonly ICardInstallmentRepository _installmentRepository;
        private readonly ICreditCardRepository _cardRepository;
        private readonly IRegisterRepository _registerRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ICardPurchaseRepository _purchaseRepository;

        public CardPlanService(
            ICardPlanRepository planRepository,
            ICardInstallmentRepository installmentRepository,
            ICreditCardRepository cardRepository,
            IRegisterRepository registerRepository,
            IBalanceRepository balanceRepository,
            ICardPurchaseRepository purchaseRepository)
        {
            _planRepository = planRepository;
            _installmentRepository = installmentRepository;
            _cardRepository = cardRepository;
            _registerRepository = registerRepository;
            _balanceRepository = balanceRepository;
            _purchaseRepository = purchaseRepository;
        }

        public async Task<IEnumerable<CardPlanDto>> GetByCardAsync(Guid cardId)
        {
            var plans = await _planRepository.GetByCardAsync(cardId);
            return plans.Select(CardPlanMapper.ToDto);
        }

        public async Task<Guid> CreateAsync(CardPlanCreateDto dto)
        {
            var card = await _cardRepository.GetByIdAsync(dto.CardId);
            if (card is null)
                throw new ArgumentException("Credit card not found.", nameof(dto.CardId));

            if (!Enum.TryParse<Category>(dto.Category, true, out var category))
                category = Category.Others;

            if (!Enum.TryParse<Month>(dto.StartMonth, true, out var startMonth))
                throw new ArgumentException("Invalid start month.", nameof(dto.StartMonth));

            // Auto-calculate remaining balance if not provided
            var remainingBalance = dto.RemainingBalance
                ?? Math.Round(dto.MonthlyAmount * (dto.TotalInstallments - dto.CurrentInstallment + 1), 2);

            if (remainingBalance <= 0)
                remainingBalance = dto.MonthlyAmount;

            var plan = CardPlan.Create(
                dto.CardId,
                dto.AccountId ?? card.AccountId,
                dto.Description,
                category,
                dto.OriginalAmount,
                dto.MonthlyAmount,
                dto.TotalInstallments,
                dto.CurrentInstallment,
                remainingBalance,
                dto.DueDay,
                startMonth,
                dto.StartYear
            );

            await _planRepository.AddAsync(plan);
            await _planRepository.SaveChangesAsync();

            return plan.Id;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan is null) return false;

            plan.SoftDelete();
            await _planRepository.UpdateAsync(plan);
            await _planRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CardInstallmentDto>> GetOrGenerateInstallmentsForMonthAsync(
            Guid cardId, string monthName, int year, int closingDay)
        {
            if (!Enum.TryParse<Month>(monthName, true, out var monthEnum))
                throw new ArgumentException("Invalid month name.", nameof(monthName));

            int monthNumber = (int)monthEnum;
            var billingMonth = new DateTime(year, monthNumber, 1);

            // Get all active plans for this card
            var activePlans = await _planRepository.GetActiveByCardAsync(cardId);

            var card = await _cardRepository.GetByIdAsync(cardId);
            var hasLinkedAccount = card?.AccountId.HasValue ?? false;
            var accountId = card?.AccountId ?? Guid.Empty;

            foreach (var plan in activePlans)
            {
                // Calculate which installment number falls on this billing month
                int planInstallmentForThisMonth = CalculateInstallmentForMonth(plan, monthNumber, year);
                if (planInstallmentForThisMonth < 1 || planInstallmentForThisMonth > plan.TotalInstallments)
                    continue;

                // Skip generating if this installment has already been paid/processed
                if (planInstallmentForThisMonth <= plan.CurrentInstallment)
                    continue;

                // Check if an installment for this plan+month already exists
                // We identify plan-generated installments by a special CardPurchaseId sentinel
                // Actually, CardPlan generates CardInstallments linked via a "virtual" CardPurchase placeholder.
                // To avoid coupling, we check existence via the Register subdescription pattern.
                bool alreadyGenerated = await InstallmentAlreadyExistsForPlanAndMonth(
                    plan, planInstallmentForThisMonth, billingMonth, accountId, hasLinkedAccount);

                if (alreadyGenerated) continue;

                // Generate the CardInstallment for this plan+month
                var daysInMonth = DateTime.DaysInMonth(year, monthNumber);
                var dueDate = new DateTime(year, monthNumber, Math.Min(closingDay, daysInMonth));

                // We need a CardPurchase to link the installment — create a "plan proxy" purchase if needed
                var proxyPurchase = await EnsureProxyPurchaseExistsAsync(plan);

                var installment = CardInstallment.Create(
                    proxyPurchase.Id,
                    planInstallmentForThisMonth,
                    plan.MonthlyAmount,
                    dueDate,
                    Affirmation.No
                );

                proxyPurchase.AddInstallment(installment);
                await _installmentRepository.AddAsync(installment);

                // Create linked account register if applicable
                if (hasLinkedAccount && accountId != Guid.Empty)
                {
                    var week = Math.Clamp(CalculateCalendarWeek(dueDate), 1, 6);
                    var reg = Register.Create(
                        dueDate,
                        EventType.Debit,
                        planInstallmentForThisMonth,
                        plan.MonthlyAmount,
                        $"[Cartão] {plan.Description}",
                        monthEnum,
                        week,
                        Affirmation.No,
                        null,
                        accountId,
                        plan.Category
                    );
                    await _registerRepository.AddAsync(reg);
                }
            }

            await _installmentRepository.SaveChangesAsync();
            await _registerRepository.SaveChangesAsync();

            if (hasLinkedAccount && accountId != Guid.Empty)
            {
                await RecalculateBalanceAsync(accountId, monthEnum, year);
                await _balanceRepository.SaveChangesAsync();
            }

            // Return all installments for this card+month (CardPurchase + CardPlan)
            var installments = await _installmentRepository.GetByCardAndMonthAsync(cardId, billingMonth);
            return installments.Select(CardPurchaseMapper.ToInstallmentDto);
        }

        public async Task<CardPlanDto?> MarkInstallmentPaidAsync(Guid planId)
        {
            var plan = await _planRepository.GetByIdAsync(planId);
            if (plan is null) return null;

            plan.AdvanceInstallment();
            await _planRepository.UpdateAsync(plan);
            await _planRepository.SaveChangesAsync();

            return CardPlanMapper.ToDto(plan);
        }

        // --------------- Private helpers ---------------

        private int CalculateInstallmentForMonth(CardPlan plan, int monthNumber, int year)
        {
            // Determine how many months have elapsed since StartMonth/StartYear
            var startDate = new DateTime(plan.StartYear, (int)plan.StartMonth, 1);
            var targetDate = new DateTime(year, monthNumber, 1);
            int monthsElapsed = ((targetDate.Year - startDate.Year) * 12) + (targetDate.Month - startDate.Month);
            // Installment number at StartMonth is 1, so at monthsElapsed it's 1 + monthsElapsed
            return monthsElapsed + 1;
        }

        private async Task<bool> InstallmentAlreadyExistsForPlanAndMonth(
            CardPlan plan, int installmentNumber, DateTime billingMonth,
            Guid accountId, bool hasLinkedAccount)
        {
            if (hasLinkedAccount && accountId != Guid.Empty)
            {
                var month = (Month)billingMonth.Month;
                var registers = await _registerRepository.GetByAccountAndMonthAsync(accountId, month, billingMonth.Year);
                return registers.Any(r =>
                    !r.IsDeleted &&
                    r.Subdescription == $"[Cartão] {plan.Description}" &&
                    r.InstallmentCurrent == installmentNumber);
            }

            // Fallback: check CardInstallments for proxy purchase
            var card = await _cardRepository.GetByIdAsync(plan.CardId);
            if (card == null) return false;

            var installments = await _installmentRepository.GetByCardAndMonthAsync(plan.CardId, billingMonth);
            return installments.Any(i =>
                !i.IsDeleted &&
                i.InstallmentNumber == installmentNumber &&
                i.Amount == plan.MonthlyAmount &&
                i.CardPurchase?.Description == plan.Description);
        }

        private async Task<CardPurchase> EnsureProxyPurchaseExistsAsync(CardPlan plan)
        {
            // Look for an existing proxy CardPurchase first
            var purchases = await _purchaseRepository.GetByCardAsync(plan.CardId);
            var existing = purchases.FirstOrDefault(p =>
                !p.IsDeleted &&
                p.Description == plan.Description &&
                p.TotalInstallments == plan.TotalInstallments &&
                p.TotalAmount == plan.OriginalAmount);

            if (existing != null)
            {
                return existing;
            }

            // Create a proxy CardPurchase if it doesn't exist yet
            var proxyPurchase = CardPurchase.Create(
                plan.Description,
                new DateTime(plan.StartYear, (int)plan.StartMonth, 1, 0, 0, 0, DateTimeKind.Utc),
                plan.OriginalAmount,
                plan.TotalInstallments,
                plan.CardId,
                plan.AccountId,
                plan.Category,
                PurchaseType.Credit
            );

            await _purchaseRepository.AddAsync(proxyPurchase);
            await _purchaseRepository.SaveChangesAsync();

            return proxyPurchase;
        }

        private int CalculateCalendarWeek(DateTime date)
        {
            var firstOfMonth = new DateTime(date.Year, date.Month, 1);
            int firstDayOfWeek = (int)firstOfMonth.DayOfWeek;
            return Math.Clamp((date.Day + firstDayOfWeek - 1) / 7 + 1, 1, 6);
        }

        private async Task RecalculateBalanceAsync(Guid accountId, Month month, int year)
        {
            var registers = await _registerRepository.GetByAccountAndMonthAsync(accountId, month, year);
            decimal balanceAmount = 0m;
            foreach (var reg in registers)
            {
                if (reg.IsDeleted) continue;
                if (reg.EventType == EventType.Credit) balanceAmount += reg.Value;
                else if (reg.EventType == EventType.Debit) balanceAmount -= reg.Value;
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
    }
}
