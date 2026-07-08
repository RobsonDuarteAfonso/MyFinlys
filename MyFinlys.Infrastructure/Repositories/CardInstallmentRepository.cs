using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories
{
    public class CardInstallmentRepository : Repository<CardInstallment>, ICardInstallmentRepository
    {
        public CardInstallmentRepository(MyFinlysDbContext context) : base(context) { }

        public async Task<IEnumerable<CardInstallment>> GetByPurchaseIdAsync(Guid purchaseId)
        {
            return await _context.CardInstallments
                .Where(ci => ci.CardPurchaseId == purchaseId && !ci.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<CardInstallment>> GetByCardAndMonthAsync(Guid cardId, DateTime billingMonth)
        {
            var startOfMonth = new DateTime(billingMonth.Year, billingMonth.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

            return await _context.CardInstallments
                .Include(ci => ci.CardPurchase)
                .Where(ci => ci.CardPurchase.CardId == cardId &&
                             ci.DueDate >= startOfMonth &&
                             ci.DueDate <= endOfMonth &&
                             !ci.IsDeleted &&
                             !ci.CardPurchase.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<CardInstallment>> GetByAccountAndMonthAsync(Guid accountId, DateTime billingMonth)
        {
            var startOfMonth = new DateTime(billingMonth.Year, billingMonth.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

            return await _context.CardInstallments
                .Include(ci => ci.CardPurchase)
                .ThenInclude(cp => cp.Card)
                .Where(ci => ci.CardPurchase.Card.AccountId == accountId &&
                             ci.DueDate >= startOfMonth &&
                             ci.DueDate <= endOfMonth &&
                             !ci.IsDeleted &&
                             !ci.CardPurchase.IsDeleted)
                .ToListAsync();
        }
    }
}
