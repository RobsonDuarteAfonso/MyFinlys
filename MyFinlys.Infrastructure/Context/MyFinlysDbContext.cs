using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Infrastructure.Context
{
    public class MyFinlysDbContext : DbContext
    {
        public MyFinlysDbContext(DbContextOptions<MyFinlysDbContext> options) : base(options)
        {
        }

        // Apenas declara os DbSets necessários
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Register> Registers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Balance> Balances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica todos os mapeamentos via IEntityTypeConfiguration
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyFinlysDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
