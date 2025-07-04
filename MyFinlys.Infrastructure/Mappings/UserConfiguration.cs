using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Infrastructure.Mappings
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            // Chave primária
            builder.HasKey(u => u.Id);

            // Campos padrão de Entity
            builder.Property(u => u.CreatedAt)
                   .IsRequired();

            builder.Property(u => u.UpdatedAt)
                   .IsRequired();

            // Nome
            builder.Property(u => u.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // Email (Value Object armazenado como string)
            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value)
                     .HasColumnName("Email")
                     .IsRequired()
                     .HasMaxLength(150);
            });

            // Password (Value Object armazenado como string)
            builder.OwnsOne(u => u.Password, password =>
            {
                password.Property(p => p.Value)
                        .HasColumnName("Password")
                        .IsRequired()
                        .HasMaxLength(100);
            });

            // Relacionamento com UserAccounts (muitos-para-muitos via entidade de junção)
            builder.HasMany(u => u.UserAccounts)
                   .WithOne(ua => ua.User)
                   .HasForeignKey(ua => ua.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
