using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface ICreditCardRepository : IRepository<CreditCard>
{
    Task<IEnumerable<CreditCard>> GetByAccountAsync(Guid accountId);
    Task<IEnumerable<CreditCard>> GetByUserIdAsync(Guid userId);
}
