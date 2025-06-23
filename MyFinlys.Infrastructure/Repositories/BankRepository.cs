using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;

namespace MyFinlys.Infrastructure.Repositories
{
    public class BankRepository : Repository<Bank>, IBankRepository
    {
        public BankRepository(MyFinlysDbContext context) : base(context) { }

        public async Task<Bank?> GetByNaneAsync(string bankName)
        {
            return await _context.Banks.FirstOrDefaultAsync(b => b.Name == bankName);
        }
    }
}
