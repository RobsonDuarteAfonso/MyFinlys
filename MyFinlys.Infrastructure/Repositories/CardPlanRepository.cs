using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories
{
    public class CardPlanRepository : Repository<CardPlan>, ICardPlanRepository
    {
        public CardPlanRepository(MyFinlysDbContext context) : base(context) { }

        public async Task<IEnumerable<CardPlan>> GetByCardAsync(Guid cardId)
        {
            return await _context.CardPlans
                .Where(cp => cp.CardId == cardId && !cp.IsDeleted)
                .OrderByDescending(cp => cp.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CardPlan>> GetActiveByCardAsync(Guid cardId)
        {
            return await _context.CardPlans
                .Where(cp => cp.CardId == cardId && !cp.IsDeleted
                          && cp.CurrentInstallment <= cp.TotalInstallments
                          && cp.RemainingBalance > 0)
                .ToListAsync();
        }
    }
}
