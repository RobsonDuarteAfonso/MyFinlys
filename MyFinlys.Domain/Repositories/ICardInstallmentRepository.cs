using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface ICardInstallmentRepository : IRepository<CardInstallment>
{
    Task<IEnumerable<CardInstallment>> GetByPurchaseIdAsync(Guid purchaseId);
    Task<IEnumerable<CardInstallment>> GetByCardAndMonthAsync(Guid cardId, DateTime billingMonth);
    Task<IEnumerable<CardInstallment>> GetByAccountAndMonthAsync(Guid accountId, DateTime billingMonth);
}
