using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces
{
    public interface ICardPurchaseService
    {
        Task<CardPurchaseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<CardPurchaseDto>> GetByCardAsync(Guid cardId);
        Task<IEnumerable<CardInstallmentDto>> GetInstallmentsByCardAndMonthAsync(Guid cardId, string monthName, int year);
        Task<Guid> CreateAsync(CardPurchaseCreateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<CardInstallmentDto?> UpdateInstallmentRealizedAsync(Guid installmentId, string realized);
        Task AdjustClosingDayAsync(Guid cardId, string monthName, int year, int closingDay);
        Task<IEnumerable<CardPurchasePlanDto>> GetPlansByCardAsync(Guid cardId);
        Task<bool> TransferInstallmentToNextMonthAsync(Guid installmentId);
    }
}
