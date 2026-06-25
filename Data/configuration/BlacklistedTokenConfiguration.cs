using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Herfa_back.Data.configuration
{
    public class BlacklistedTokenConfiguration : IEntityTypeConfiguration<BlacklistedToken>
    {
        public void Configure(EntityTypeBuilder<BlacklistedToken> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Jti)
                .IsRequired();

            entity.HasIndex(e => e.Jti)
                .IsUnique();

            entity.Property(e => e.BlacklistedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
