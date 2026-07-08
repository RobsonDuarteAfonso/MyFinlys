using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces
{
    public interface ICardPlanService
    {
        Task<IEnumerable<CardPlanDto>> GetByCardAsync(Guid cardId);

        Task<Guid> CreateAsync(CardPlanCreateDto dto);

        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Returns all card installment entries for the given card+month.
        /// For each active CardPlan that does not yet have a CardInstallment in this month,
        /// one is created automatically (and linked to the account register if applicable).
        /// </summary>
        Task<IEnumerable<CardInstallmentDto>> GetOrGenerateInstallmentsForMonthAsync(
            Guid cardId, string monthName, int year, int closingDay);

        /// <summary>
        /// Marks the CardPlan's current installment as paid, advances the counter and
        /// recalculates the remaining balance.
        /// </summary>
        Task<CardPlanDto?> MarkInstallmentPaidAsync(Guid planId);
    }
}
