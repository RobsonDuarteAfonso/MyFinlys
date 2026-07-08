using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Infrastructure.Mappings
{
    public class CardInstallmentConfiguration : IEntityTypeConfiguration<CardInstallment>
    {
        public void Configure(EntityTypeBuilder<CardInstallment> builder)
        {
            builder.ToTable("CardInstallments");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.CardPurchaseId)
                   .IsRequired();

            builder.Property(ci => ci.InstallmentNumber)
                   .IsRequired();

            builder.Property(ci => ci.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(ci => ci.DueDate)
                   .IsRequired();

            builder.Property(ci => ci.Realized)
                   .HasConversion<string>()
                   .IsRequired();
        }
    }
}
