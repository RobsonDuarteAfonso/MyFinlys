using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface ICardPurchaseRepository : IRepository<CardPurchase>
{
    Task<IEnumerable<CardPurchase>> GetByCardAsync(Guid cardId);
}
