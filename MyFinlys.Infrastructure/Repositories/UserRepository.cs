using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(MyFinlysDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.Value.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<Account>> GetAccountsAsync(Guid userId)
        {
            return await _context.UserAccounts
                .Where(ua => ua.UserId == userId)
                .Include(ua => ua.Account)
                .Select(ua => ua.Account)
                .ToListAsync();
        }
    }
}
