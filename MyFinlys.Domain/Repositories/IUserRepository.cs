using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<Account>> GetAccountsAsync(Guid userId);
}
