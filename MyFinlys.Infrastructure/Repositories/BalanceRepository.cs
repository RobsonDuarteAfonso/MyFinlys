using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;
using MyFinlys.Domain.ValueObjects;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories
{
    public class BalanceRepository : Repository<Balance>, IBalanceRepository
    {
        public BalanceRepository(MyFinlysDbContext context) : base(context) { }

        public async Task<Balance?> GetByAccountMonthYearAsync(Guid accountId, Month month, Year year)
        {
            return await _context.Balances
                .FirstOrDefaultAsync(b =>
                    b.AccountId == accountId &&
                    b.Month == month &&
                    b.Year == year
                );
        }

        public async Task<IEnumerable<Balance>> GetByAccountAsync(Guid accountId)
        {
            return await _context.Balances
                .Where(b => b.AccountId == accountId)
                .ToListAsync();
        }
    }
}
