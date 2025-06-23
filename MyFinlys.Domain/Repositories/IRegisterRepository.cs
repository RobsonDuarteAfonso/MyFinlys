using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Domain.Repositories;

public interface IRegisterRepository : IRepository<Register>
{
    Task<IEnumerable<Register>> GetByEventIdAsync(Guid eventId);
    Task<IEnumerable<Register>> GetByMonthAsync(Month month);
    Task<IEnumerable<Register>> GetByMonthAndWeekAsync(Month month, int week);
}
