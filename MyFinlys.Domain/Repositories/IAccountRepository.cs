using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId);
}

