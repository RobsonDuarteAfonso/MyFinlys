using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories
{
    public class RegisterRepository : Repository<Register>, IRegisterRepository
    {
        public RegisterRepository(MyFinlysDbContext context) : base(context) { }

        public async Task<IEnumerable<Register>> GetByEventIdAsync(Guid eventId)
        {
            return await _context.Registers
                .Where(r => r.EventId == eventId)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Register>> GetByMonthAsync(Month month)
        {
            return await _context.Registers
                .Where(r => r.Month == month)
                .ToListAsync();
        }

        public async Task<IEnumerable<Register>> GetByMonthAndWeekAsync(Month month, int week)
        {
            return await _context.Registers
                .Where(r => r.Month == month && r.Week == week)
                .ToListAsync();
        }
    }
}
