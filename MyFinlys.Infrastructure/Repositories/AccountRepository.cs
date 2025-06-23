using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(MyFinlysDbContext context) : base(context) { }

        public async Task<Account?> GetByNumberAsync(string number)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Number == number);
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
