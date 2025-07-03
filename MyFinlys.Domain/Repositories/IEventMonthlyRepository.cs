using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface IEventMonthlyRepository : IRepository<EventMonthly>
{
    Task<IEnumerable<EventMonthly>> GetByDueDateAsync(DateTime dueDate);
    Task<IEnumerable<EventMonthly>> GetByMonthAsync(int year, int month);
    Task<IEnumerable<EventMonthly>> GetUpcomingEventsAsync(Guid accountId, DateTime untilDate);
}
