using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<IEnumerable<User>> GetUsersAsync(Guid accountId);
}

