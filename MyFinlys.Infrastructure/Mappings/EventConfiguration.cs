using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Infrastructure.Mappings
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt).IsRequired();

            builder.Property(e => e.Type)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(e => e.Period)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(e => e.Value)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(e => e.AutoRealized)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(e => e.Finished)
                   .HasConversion<string>()
                   .IsRequired();

            builder.OwnsOne(e => e.Installment, installment =>
            {
                installment.Property(i => i.InstallmentTotal)
                           .HasColumnName("InstallmentTotal")
                           .IsRequired();

                installment.Property(i => i.InstallmentCurrent)
                           .HasColumnName("InstallmentCurrent")
                           .IsRequired();

                installment.Property(i => i.InstallmentValue)
                           .HasColumnName("InstallmentValue")
                           .HasColumnType("decimal(18,2)")
                           .IsRequired();

                installment.Property(i => i.DateInitial)
                           .HasColumnName("InstallmentDateInitial")
                           .IsRequired(false);

                installment.Property(i => i.DateFinish)
                           .HasColumnName("InstallmentDateFinish")
                           .IsRequired(false);
            });

            builder.HasOne(e => e.Account)
                   .WithMany()
                   .HasForeignKey(e => e.AccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasDiscriminator<EventPeriod>("Period")
                   .HasValue<EventWeekly>(EventPeriod.Weekly)
                   .HasValue<EventMonthly>(EventPeriod.Monthly)
                   .HasValue<EventBiweekly>(EventPeriod.Biweekly);

            builder.Property<DayOfWeek>(nameof(EventWeekly.DayOfWeek))
                   .HasConversion<string>()
                   .HasColumnName("DayOfWeek");

            builder.Property<DateTime>(nameof(EventBiweekly.StartDate))
                   .HasColumnName("StartDate");

            builder.Property<DateTime>(nameof(EventMonthly.Due))
                   .HasColumnName("Due");
        }
    }
}
