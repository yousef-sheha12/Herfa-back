using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Herfa_back.Data.configuration
{
    public class ArtisanProfileConfiguration : IEntityTypeConfiguration<ArtisanProfile>
    {
        public void Configure(EntityTypeBuilder<ArtisanProfile> AP)
        {
            // ArtisanProfile → Category (Many-to-One)
            AP.HasOne(a => a.Category)
                .WithMany(c => c.Artisans)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // ArtisanProfile → User (One-to-One)
            AP.HasOne(a => a.User)
                .WithOne(u => u.ArtisanProfile)
                .HasForeignKey<ArtisanProfile>(a => a.UserId);

            // ArtisanProfile properties
            AP.Property(a => a.NationalId)
                .IsRequired()
                .HasMaxLength(15);

            //Propertys
            // Indexes for efficient querying
            AP.HasIndex(a => a.City);
            AP.HasIndex(a => a.CategoryId);
        }
    }
}
