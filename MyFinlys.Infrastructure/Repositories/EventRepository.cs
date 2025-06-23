using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(MyFinlysDbContext context) : base(context) { }

        public async Task<IEnumerable<Event>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Events
                .Where(e => e.AccountId == accountId)
                .ToListAsync();
        }
    }
}
