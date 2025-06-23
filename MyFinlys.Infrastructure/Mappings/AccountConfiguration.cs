using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Infrastructure.Mappings
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            // Chave primária
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Number)
                   .IsRequired()
                   .HasMaxLength(20);

            // Enum AccountType salvo como string
            builder.Property(a => a.Type)
                   .HasConversion<string>()
                   .IsRequired();

            // Relacionamento com Bank (com navegação inversa)
            builder.HasOne(a => a.Bank)
                   .WithMany(b => b.Accounts)
                   .HasForeignKey(a => a.BankId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com UserAccounts
            builder.HasMany(a => a.UserAccounts)
                   .WithOne(ua => ua.Account)
                   .HasForeignKey(ua => ua.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento com Balances
            builder.HasMany(a => a.Balances)
                   .WithOne(b => b.Account)
                   .HasForeignKey(b => b.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
