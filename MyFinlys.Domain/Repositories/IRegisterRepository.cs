using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Repositories;

public interface IRegisterRepository : IRepository<Register>
{
    Task<IEnumerable<Register>> GetByEventIdAsync(Guid eventId);
    Task<IEnumerable<Register>> GetByAccountIdAsync(Guid accountId);
    Task<IEnumerable<Register>> GetByMonthAsync(Month month);
    Task<IEnumerable<Register>> GetByMonthAndWeekAsync(Month month, int week);
    Task<IEnumerable<Register>> GetByAccountAndMonthAsync(Guid accountId, Month month, int year);
    Task<IEnumerable<Guid>> GetEventIdsForMonthIncludingDeletedAsync(Guid accountId, Month month, int year);
    Task PhysicalDeleteAsync(Guid id);
}

