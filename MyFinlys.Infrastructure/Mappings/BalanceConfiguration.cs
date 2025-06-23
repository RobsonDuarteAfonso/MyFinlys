using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.ValueObjects;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Infrastructure.Mappings
{
    public class BalanceConfiguration : IEntityTypeConfiguration<Balance>
    {
        public void Configure(EntityTypeBuilder<Balance> builder)
        {
            builder.ToTable("Balances");

            builder.HasKey(b => b.Id);

            // Campos padrão de Entity
            builder.Property(b => b.CreatedAt)
                   .IsRequired();

            builder.Property(b => b.UpdatedAt)
                   .IsRequired();

            // Relacionamento com Account
            builder.HasOne(b => b.Account)
                   .WithMany(a => a.Balances)
                   .HasForeignKey(b => b.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Enum Month salvo como int
            builder.Property(b => b.Month)
                   .HasConversion<int>()
                   .IsRequired();

            // Value Object Year
            builder.Property(b => b.Year)
                   .HasConversion(
                       y => y.Value, // para o banco
                       v => Year.Create(v) // do banco
                   )
                   .IsRequired();

            builder.Property(b => b.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();
        }
    }
}
