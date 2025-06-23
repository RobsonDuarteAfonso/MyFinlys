using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Domain.Repositories;

public interface IBalanceRepository : IRepository<Balance>
{
    Task<Balance?> GetByAccountMonthYearAsync(Guid accountId, Month month, Year year);
    Task<IEnumerable<Balance>> GetByAccountAsync(Guid accountId);
}