using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface IEventWeeklyRepository : IRepository<EventWeekly>
{
    Task<IEnumerable<EventWeekly>> GetByDayOfWeekAsync(DayOfWeek dayOfWeek);
    Task<IEnumerable<EventWeekly>> GetByAccountIdAndDayAsync(Guid accountId, DayOfWeek dayOfWeek);

}
