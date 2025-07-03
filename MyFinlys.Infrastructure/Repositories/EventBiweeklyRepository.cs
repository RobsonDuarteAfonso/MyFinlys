using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories;

public class EventBiweeklyRepository : Repository<EventBiweekly>, IEventBiweeklyRepository
{
    public EventBiweeklyRepository(MyFinlysDbContext context) : base(context) { }

    public async Task<IEnumerable<EventBiweekly>> GetByStartDateAsync(DateTime startDate)
    {
        return await _context.Events.OfType<EventBiweekly>()
            .Where(e => e.StartDate.Date == startDate.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventBiweekly>> GetByAccountIdAsync(Guid accountId)
    {
        return await _context.Events.OfType<EventBiweekly>()
            .Where(e => e.AccountId == accountId)
            .ToListAsync();
    }

    public async Task<IEnumerable<EventBiweekly>> GetByDayOfWeekAsync(DayOfWeek dayOfWeek)
    {
        return await _context.Events.OfType<EventBiweekly>()
            .Where(e => e.DayOfWeek == dayOfWeek)
            .ToListAsync();
    }
}
