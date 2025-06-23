using MyFinlys.Domain.Entities;

namespace MyFinlys.Domain.Repositories;

public interface IBankRepository : IRepository<Bank>
{
    Task<Bank?> GetByNaneAsync(string bankName);
}
