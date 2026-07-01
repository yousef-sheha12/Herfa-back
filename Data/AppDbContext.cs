using Herfa_back.Models;
using Microsoft.EntityFrameworkCore;
namespace Herfa_back.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // Person-4
        public DbSet<Job> Jobs { get; set; }



        //Person 2 : Mohamed Samy
        public DbSet<ArtisanProfile> ArtisanProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }

        //Person3 : Dai 
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<ServiceOffer> ServiceOffers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
        

    }
}
