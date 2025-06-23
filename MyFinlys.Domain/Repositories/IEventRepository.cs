using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface IEventRepository : IRepository<Event>
{
    Task<IEnumerable<Event>> GetByAccountIdAsync(Guid accountId);
}
