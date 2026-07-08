using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories
{
    public class CreditCardRepository : Repository<CreditCard>, ICreditCardRepository
    {
        public CreditCardRepository(MyFinlysDbContext context) : base(context) { }

        public async Task<IEnumerable<CreditCard>> GetByAccountAsync(Guid accountId)
        {
            return await _context.CreditCards
                .Where(cc => cc.AccountId == accountId && !cc.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<CreditCard>> GetByUserIdAsync(Guid userId)
        {
            return await _context.CreditCards
                .Where(cc => cc.UserId == userId && !cc.IsDeleted)
                .ToListAsync();
        }
    }
}
