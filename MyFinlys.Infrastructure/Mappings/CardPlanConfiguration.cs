using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Infrastructure.Mappings
{
    public class CardPlanConfiguration : IEntityTypeConfiguration<CardPlan>
    {
        public void Configure(EntityTypeBuilder<CardPlan> builder)
        {
            builder.ToTable("CardPlans");

            builder.HasKey(cp => cp.Id);

            builder.Property(cp => cp.Description)
                   .HasMaxLength(300)
                   .IsRequired();

            builder.Property(cp => cp.Category)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(cp => cp.OriginalAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(cp => cp.MonthlyAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(cp => cp.TotalInstallments)
                   .IsRequired();

            builder.Property(cp => cp.CurrentInstallment)
                   .IsRequired();

            builder.Property(cp => cp.RemainingBalance)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(cp => cp.DueDay)
                   .IsRequired();

            builder.Property(cp => cp.StartMonth)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(cp => cp.StartYear)
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

            // IsActive is computed — do not map to column
            builder.Ignore(cp => cp.IsActive);
        }
    }
}
