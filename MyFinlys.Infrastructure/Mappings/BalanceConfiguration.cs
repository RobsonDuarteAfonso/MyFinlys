using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.ValueObjects;

namespace MyFinlys.Infrastructure.Mappings
{
    public class BalanceConfiguration : IEntityTypeConfiguration<Balance>
    {
        public void Configure(EntityTypeBuilder<Balance> builder)
        {
            builder.ToTable("Balances");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.CreatedAt)
                   .IsRequired();

            builder.Property(b => b.UpdatedAt)
                   .IsRequired();

            builder.HasOne(b => b.Account)
                   .WithMany(a => a.Balances)
                   .HasForeignKey(b => b.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(b => b.Month)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(b => b.Year)
                   .HasConversion(
                       y => y.Value,
                       v => Year.Create(v)
                   )
                   .IsRequired();

            builder.Property(b => b.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();
        }
    }
}
