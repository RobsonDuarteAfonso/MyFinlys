using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(MyFinlysDbContext context) : base(context) { }

        public override async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _context.Accounts
                .AsTracking()
                .Include(a => a.Bank)
                .Include(a => a.UserAccounts)
                    .ThenInclude(ua => ua.User)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Accounts
                .Where(a => a.UserAccounts.Any(ua => ua.UserId == userId) && !a.IsDeleted)
                .Include(a => a.Bank)
                .Include(a => a.UserAccounts)
                    .ThenInclude(ua => ua.User)
                .ToListAsync();
        }

        public async Task<Account?> GetByNumberAsync(string number)
        {
            return await _context.Accounts
                .Include(a => a.UserAccounts)
                    .ThenInclude(ua => ua.User)
                .Include(a => a.Bank)
                .FirstOrDefaultAsync(a => a.Number == number && !a.IsDeleted);
        }

        public async Task<IEnumerable<User>> GetUsersAsync(Guid accountId)
        {
            return await _context.UserAccounts
                .Where(ua => ua.AccountId == accountId)
                .Include(ua => ua.User)
                .Select(ua => ua.User)
                .ToListAsync();
        }
    }
}
