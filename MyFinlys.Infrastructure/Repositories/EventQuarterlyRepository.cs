using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories;

public class EventQuarterlyRepository : Repository<EventQuarterly>, IEventQuarterlyRepository
{
    public EventQuarterlyRepository(MyFinlysDbContext context) : base(context) { }
}
