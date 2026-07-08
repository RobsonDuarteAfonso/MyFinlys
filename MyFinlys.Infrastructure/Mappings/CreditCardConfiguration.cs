using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Infrastructure.Mappings
{
    public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
    {
        public void Configure(EntityTypeBuilder<CreditCard> builder)
        {
            builder.ToTable("CreditCards");

            builder.HasKey(cc => cc.Id);

            builder.Property(cc => cc.Name)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(cc => cc.Limit)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(cc => cc.ClosingDay)
                   .IsRequired();

            builder.Property(cc => cc.DueDay)
                   .IsRequired();

            builder.Property(cc => cc.AccountId)
                   .IsRequired(false);

            builder.HasOne(cc => cc.Account)
                   .WithMany()
                   .HasForeignKey(cc => cc.AccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(cc => cc.UserId)
                   .IsRequired();

            builder.HasOne(cc => cc.User)
                   .WithMany()
                   .HasForeignKey(cc => cc.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
