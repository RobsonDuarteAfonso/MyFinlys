using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories;

public class EventAnnualRepository : Repository<EventAnnual>, IEventAnnualRepository
{
    public EventAnnualRepository(MyFinlysDbContext context) : base(context) { }
}
