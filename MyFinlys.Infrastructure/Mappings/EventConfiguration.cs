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

            // Chave primária
            builder.HasKey(e => e.Id);

            // Campos padrão de Entity
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt).IsRequired();

            // Enum EventType salvo como string
            builder.Property(e => e.Type)
                   .HasConversion<string>()
                   .IsRequired();

            // Enum EventPeriod salvo como string (discriminador)
            builder.Property(e => e.Period)
                   .HasConversion<string>()
                   .IsRequired();

            // Valor decimal
            builder.Property(e => e.Value)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            // Descrição
            builder.Property(e => e.Description)
                   .HasMaxLength(500)
                   .IsRequired();

            // Enum Affirmation salvo como string
            builder.Property(e => e.AutoRealized)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(e => e.Finished)
                   .HasConversion<string>()
                   .IsRequired();

            // Mapeamento do Value Object Installment
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

            // Relacionamento com Account
            builder.HasOne(e => e.Account)
                   .WithMany()
                   .HasForeignKey(e => e.AccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configuração da herança TPH usando Period como discriminador
            builder.HasDiscriminator<EventPeriod>("Period")
                   .HasValue<EventWeekly>(EventPeriod.Weekly)
                   .HasValue<EventMonthly>(EventPeriod.Monthly)
                   .HasValue<EventBiweekly>(EventPeriod.Biweekly);

            // Propriedades específicas das subclasses

            // EventWeekly e EventBiweekly: DayOfWeek (enum como string)
            builder.Property(nameof(EventWeekly.DayOfWeek))
                   .HasConversion<string>()
                   .HasColumnName("DayOfWeek");

            // EventBiweekly: StartDate
            builder.Property(nameof(EventBiweekly.StartDate))
                   .HasColumnName("StartDate");

            // EventMonthly: Due
            builder.Property(nameof(EventMonthly.Due))
                   .HasColumnName("Due");
        }
    }
}
