using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Infrastructure.Mappings
{
    public class BankConfiguration : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.ToTable("Banks");

            // Chave primária
            builder.HasKey(b => b.Id);

            // Campos padrão de Entity
            builder.Property(b => b.CreatedAt)
                   .IsRequired();

            builder.Property(b => b.UpdatedAt)
                   .IsRequired();

            // Nome do banco
            builder.Property(b => b.Name)
                   .IsRequired()
                   .HasMaxLength(100);
        }
    }
}
