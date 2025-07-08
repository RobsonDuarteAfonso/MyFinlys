using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinlys.Domain.Entities;

namespace MyFinlys.Infrastructure.Mappings
{
    public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
    {
        public void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            builder.ToTable("UserAccounts");

            builder.HasKey(ua => new { ua.UserId, ua.AccountId });

            builder.HasOne(ua => ua.User)
                   .WithMany(u => u.UserAccounts)
                   .HasForeignKey(ua => ua.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ua => ua.Account)
                   .WithMany(a => a.UserAccounts)
                   .HasForeignKey(ua => ua.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
