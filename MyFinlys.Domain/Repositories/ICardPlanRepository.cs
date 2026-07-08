using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface ICardPlanRepository : IRepository<CardPlan>
{
    Task<IEnumerable<CardPlan>> GetByCardAsync(Guid cardId);
    Task<IEnumerable<CardPlan>> GetActiveByCardAsync(Guid cardId);
}
