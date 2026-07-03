using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Herfa_back.Data.Configrations
{
    public class ServiceOfferConfiguration : IEntityTypeConfiguration<ServiceOffer>
    {

        public void Configure(EntityTypeBuilder<ServiceOffer> builder)
        {
            builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(1000);

            builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.ServiceRequest)
            .WithMany(x => x.Offers)
            .HasForeignKey(x => x.ServiceRequestId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Artisan)
            .WithMany(x => x.Offers)
            .HasForeignKey(x => x.ArtisanId)
            .OnDelete(DeleteBehavior.Restrict);

            // يمنع الحرفي من تقديم عرضين لنفس الطلب
            builder.HasIndex(x => new
            {
                x.ServiceRequestId,
                x.ArtisanId
            })
             .IsUnique();

            builder.Property(x => x.Status).HasConversion<string>();
        }
    }
    
}
