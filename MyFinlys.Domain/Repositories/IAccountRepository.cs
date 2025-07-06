using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<User>> GetUsersAsync(Guid accountId);
    Task<Account?> GetByNumberAsync(string number);
}

