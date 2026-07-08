using Microsoft.EntityFrameworkCore;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Infrastructure.Context
{
    public class MyFinlysDbContext : DbContext
    {
        public MyFinlysDbContext(DbContextOptions<MyFinlysDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Register> Registers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<CardPurchase> CardPurchases { get; set; }
        public DbSet<CardInstallment> CardInstallments { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<CardPlan> CardPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyFinlysDbContext).Assembly);

            // Map properties of derived entity types to their respective columns instead of shadow properties on base class
            modelBuilder.Entity<EventWeekly>()
                .Property(e => e.DayOfWeek)
                .HasConversion<string>()
                .HasColumnName("DayOfWeek");

            modelBuilder.Entity<EventBiweekly>(b =>
            {
                b.Property(e => e.DayOfWeek)
                 .HasConversion<string>()
                 .HasColumnName("DayOfWeek");
                 
                b.Property(e => e.StartDate)
                 .HasColumnName("StartDate");
            });

            modelBuilder.Entity<EventMonthly>()
                .Property(e => e.Due)
                .HasColumnName("Due");

            modelBuilder.Entity<EventQuarterly>()
                .Property(e => e.Due)
                .HasColumnName("Due");

            modelBuilder.Entity<EventSemiAnnual>()
                .Property(e => e.Due)
                .HasColumnName("Due");

            modelBuilder.Entity<EventAnnual>()
                .Property(e => e.Due)
                .HasColumnName("Due");

            base.OnModelCreating(modelBuilder);
        }
    }
}
