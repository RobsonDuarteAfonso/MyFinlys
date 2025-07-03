using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface IEventBiweeklyRepository : IRepository<EventBiweekly>
{
    Task<IEnumerable<EventBiweekly>> GetByStartDateAsync(DateTime startDate);
    Task<IEnumerable<EventBiweekly>> GetByAccountIdAsync(Guid accountId);
    Task<IEnumerable<EventBiweekly>> GetByDayOfWeekAsync(DayOfWeek dayOfWeek);
}
