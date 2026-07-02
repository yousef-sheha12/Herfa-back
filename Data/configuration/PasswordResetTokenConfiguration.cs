using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Herfa_back.Data.configuration
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Email)
                .IsRequired();

            entity.Property(e => e.Token)
                .IsRequired();

            entity.HasIndex(e => e.Token)
                .IsUnique();

            entity.Property(e => e.ExpiresAt)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
