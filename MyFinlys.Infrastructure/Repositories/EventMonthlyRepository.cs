using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories;

public class EventMonthlyRepository : Repository<EventMonthly>, IEventMonthlyRepository
{
    public EventMonthlyRepository(MyFinlysDbContext context) : base(context) { }

    public async Task<IEnumerable<EventMonthly>> GetByDueDateAsync(DateTime dueDate)
    {
        return await _context.Events.OfType<EventMonthly>()
            .Where(e => e.Due.Date == dueDate.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventMonthly>> GetByMonthAsync(int year, int month)
    {
        return await _context.Events.OfType<EventMonthly>()
            .Where(e => e.Due.Year == year && e.Due.Month == month)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventMonthly>> GetUpcomingEventsAsync(Guid accountId, DateTime untilDate)
    {
        return await _context.Events.OfType<EventMonthly>()
            .Where(e => e.AccountId == accountId && e.Due <= untilDate)
            .OrderBy(e => e.Due)
            .ToListAsync();
    }
}
