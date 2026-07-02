using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Herfa_back.Data.configuration
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> entity)
        {
            entity.HasKey(r => r.Id);

            entity.HasIndex(r => r.JobId).IsUnique();

            entity.Property(r => r.Rating)
                .IsRequired();

            entity.Property(r => r.Comment)
                .IsRequired(false)
                .HasMaxLength(1000);

            entity.HasOne(r => r.Job)
                .WithOne(j => j.Review)
                .HasForeignKey<Review>(r => r.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(r => r.Client)
                .WithMany()
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(r => r.Artisan)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.ArtisanId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
