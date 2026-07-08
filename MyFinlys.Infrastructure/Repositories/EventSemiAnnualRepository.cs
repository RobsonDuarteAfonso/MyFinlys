using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories;

public class EventSemiAnnualRepository : Repository<EventSemiAnnual>, IEventSemiAnnualRepository
{
    public EventSemiAnnualRepository(MyFinlysDbContext context) : base(context) { }
}
