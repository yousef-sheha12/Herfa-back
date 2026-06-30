using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Herfa_back.Data.Configrations
{
    public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
    {
        public void Configure(EntityTypeBuilder<ServiceRequest> builder)
        {
            builder.Property(x => x.Title)
          .IsRequired()
          .HasMaxLength(200);

            builder.Property(x => x.Description)
            .IsRequired();

            builder.Property(x => x.Address)
           .IsRequired()
           .HasMaxLength(500);

            builder.HasOne(x => x.Client)
            .WithMany(x => x.ServiceRequests)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Category)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Offers)
           .WithOne(x => x.ServiceRequest)
           .HasForeignKey(x => x.ServiceRequestId)
           .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Job)
           .WithOne(x => x.ServiceRequest)
           .HasForeignKey<Job>(x => x.ServiceRequestId)
           .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Status).HasConversion<string>();
        }
    }
}
