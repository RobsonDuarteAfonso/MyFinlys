using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories;

public class EventWeeklyRepository : Repository<EventWeekly>, IEventWeeklyRepository
{
    public EventWeeklyRepository(MyFinlysDbContext context) : base(context) { }

    public async Task<IEnumerable<EventWeekly>> GetByDayOfWeekAsync(DayOfWeek dayOfWeek)
    {
        return await _context.Events.OfType<EventWeekly>()
            .Where(e => e.DayOfWeek == dayOfWeek)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventWeekly>> GetByAccountIdAndDayAsync(Guid accountId, DayOfWeek dayOfWeek)
    {
        return await _context.Events.OfType<EventWeekly>()
            .Where(e => e.AccountId == accountId && e.DayOfWeek == dayOfWeek)
            .ToListAsync();
    }
}
