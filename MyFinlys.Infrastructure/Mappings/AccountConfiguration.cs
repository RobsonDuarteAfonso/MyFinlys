using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Infrastructure.Mappings
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Number)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(a => a.Type)
                   .HasConversion<string>()
                   .IsRequired();

            builder.HasOne(a => a.Bank)
                   .WithMany(b => b.Accounts)
                   .HasForeignKey(a => a.BankId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.UserAccounts)
                   .WithOne(ua => ua.Account)
                   .HasForeignKey(ua => ua.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.Balances)
                   .WithOne(b => b.Account)
                   .HasForeignKey(b => b.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
