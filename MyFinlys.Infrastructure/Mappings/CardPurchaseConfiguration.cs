using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Infrastructure.Mappings
{
    public class CardPurchaseConfiguration : IEntityTypeConfiguration<CardPurchase>
    {
        public void Configure(EntityTypeBuilder<CardPurchase> builder)
        {
            builder.ToTable("CardPurchases");

            builder.HasKey(cp => cp.Id);

            builder.Property(cp => cp.Description)
                   .HasMaxLength(250)
                   .IsRequired();

            builder.Property(cp => cp.PurchaseDate)
                   .IsRequired();

            builder.Property(cp => cp.TotalAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(cp => cp.TotalInstallments)
                   .IsRequired();

            builder.Property(cp => cp.Category)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(cp => cp.Type)
                   .HasConversion<string>()
                   .HasDefaultValue(PurchaseType.Credit)
                   .IsRequired();


            builder.Property(cp => cp.CardId)
                   .IsRequired();

            builder.HasOne(cp => cp.Card)
                   .WithMany()
                   .HasForeignKey(cp => cp.CardId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(cp => cp.AccountId)
                   .IsRequired(false);

            builder.HasOne(cp => cp.Account)
                   .WithMany()
                   .HasForeignKey(cp => cp.AccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(cp => cp.Installments)
                   .WithOne(ci => ci.CardPurchase)
                   .HasForeignKey(ci => ci.CardPurchaseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
