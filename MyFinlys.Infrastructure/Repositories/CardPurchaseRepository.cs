using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories
{
    public class CardPurchaseRepository : Repository<CardPurchase>, ICardPurchaseRepository
    {
        public CardPurchaseRepository(MyFinlysDbContext context) : base(context) { }

        public async Task<IEnumerable<CardPurchase>> GetByCardAsync(Guid cardId)
        {
            return await _context.CardPurchases
                .Include(cp => cp.Installments)
                .Where(cp => cp.CardId == cardId && !cp.IsDeleted)
                .ToListAsync();
        }
    }
}
