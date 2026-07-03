using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Herfa_back.Data.configuration
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> entity)
        {
            entity.HasKey(n => n.Id);

            entity.Property(n => n.Type)
                .HasConversion<string>();

            entity.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(n => n.IsRead)
                .HasDefaultValue(false);

            entity.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            entity.HasIndex(n => new { n.UserId, n.IsRead });
        }
    }
}
