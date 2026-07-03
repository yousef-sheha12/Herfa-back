using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Herfa_back.Data.configuration
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> entity)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.StartedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.CompletedAt)
                .IsRequired(false);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            // FK columns
            entity.Property(e => e.ServiceRequestId).IsRequired();
            entity.Property(e => e.ArtisanId).IsRequired();
            
            entity.HasOne(e => e.Client)
                .WithMany()
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}