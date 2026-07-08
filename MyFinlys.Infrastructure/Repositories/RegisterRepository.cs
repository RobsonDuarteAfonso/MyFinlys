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
                .Where(r => r.EventId == eventId && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Register>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Registers
                .Where(r => r.AccountId == accountId && !r.IsDeleted)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Register>> GetByMonthAsync(Month month)
        {
            return await _context.Registers
                .Where(r => r.Month == month && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Register>> GetByMonthAndWeekAsync(Month month, int week)
        {
            return await _context.Registers
                .Where(r => r.Month == month && r.Week == week && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Register>> GetByAccountAndMonthAsync(Guid accountId, Month month, int year)
        {
            return await _context.Registers
                .Where(r => r.AccountId == accountId && r.Month == month && r.Due.Year == year && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Guid>> GetEventIdsForMonthIncludingDeletedAsync(Guid accountId, Month month, int year)
        {
            return await _context.Registers
                .Where(r => r.AccountId == accountId && r.Month == month && r.Due.Year == year && r.EventId.HasValue)
                .Select(r => r.EventId!.Value)
                .ToListAsync();
        }

        public async Task PhysicalDeleteAsync(Guid id)
        {
            var entity = await _context.Registers.FindAsync(id);
            if (entity != null)
            {
                _context.Registers.Remove(entity);
            }
        }
    }
}

