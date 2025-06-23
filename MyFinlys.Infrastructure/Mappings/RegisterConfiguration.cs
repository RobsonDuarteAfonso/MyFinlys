using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Infrastructure.Mappings
{
    public class RegisterConfiguration : IEntityTypeConfiguration<Register>
    {
        public void Configure(EntityTypeBuilder<Register> builder)
        {
            builder.ToTable("Registers");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Due)
                   .IsRequired();

            builder.Property(r => r.EventType)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(r => r.InstallmentCurrent)
                   .IsRequired();

            builder.Property(r => r.Value)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(r => r.Subdescription)
                   .HasMaxLength(250)
                   .IsRequired();

            builder.Property(r => r.Month)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(r => r.Week)
                   .IsRequired();

            builder.Property(r => r.Realized)
                   .HasConversion<string>()
                   .IsRequired();

            // Relacionamento com Event
            builder.HasOne(r => r.Event)
                   .WithMany()
                   .HasForeignKey(r => r.EventId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
