using System.Text.Json;
using System.Text.Json.Serialization;
using Herfa_back.Models;
using Herfa_back.Models.Enums;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Herfa_back.Data
{
    public class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync() || await context.Users.AnyAsync())
            {
                // Database is already seeded
                return;
            }

            var seedDataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", "seedData.json");
            
            if (!File.Exists(seedDataPath))
            {
                return;
            }

            var jsonData = await File.ReadAllTextAsync(seedDataPath);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var seedData = JsonSerializer.Deserialize<SeedDataModel>(jsonData, options);

            if (seedData == null) return;

            // 1. Seed Categories
            var categories = new List<Category>();
            if (seedData.Categories != null && seedData.Categories.Any())
            {
                foreach (var cat in seedData.Categories)
                {
                    categories.Add(new Category
                    {
                        Name = cat.Name,
                        IconUrl = cat.IconUrl,
                        Description = cat.Description,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // 2. Seed Users
            var users = new List<User>();
            if (seedData.Users != null && seedData.Users.Any())
            {
                foreach (var userDto in seedData.Users)
                {
                    users.Add(new User
                    {
                        Username = userDto.Username,
                        Email = userDto.Email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash), // PasswordHash in JSON is plain text password
                        Role = userDto.Role,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }

            // 3. Seed Artisan Profiles
            var profiles = new List<ArtisanProfile>();
            if (seedData.ArtisanProfiles != null && seedData.ArtisanProfiles.Any())
            {
                foreach (var profile in seedData.ArtisanProfiles)
                {
                    var user = await context.Users.FirstOrDefaultAsync(u => u.Email == profile.UserEmail);
                    var category = await context.Categories.FirstOrDefaultAsync(c => c.Name == profile.CategoryName);

                    if (user != null && category != null)
                    {
                        profiles.Add(new ArtisanProfile
                        {
                            UserId = user.Id,
                            CategoryId = category.Id,
                            NationalId = profile.NationalId,
                            Bio = profile.Bio,
                            City = profile.City,
                            Rating = profile.Rating,
                            TotalReviews = profile.TotalReviews,
                            IsAvailable = profile.IsAvailable,
                            IsVerified = profile.IsVerified,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
                if (profiles.Any())
                {
                    await context.ArtisanProfiles.AddRangeAsync(profiles);
                    await context.SaveChangesAsync();
                }
            }

            // 4. Seed Service Requests
            var requests = new List<ServiceRequest>();
            if (seedData.ServiceRequests != null && seedData.ServiceRequests.Any())
            {
                foreach (var req in seedData.ServiceRequests)
                {
                    var category = await context.Categories.FirstOrDefaultAsync(c => c.Name == req.CategoryName);
                    var client = await context.Users.FirstOrDefaultAsync(u => u.Email == req.ClientEmail);

                    // Parse Enum
                    Enum.TryParse<ServiceRequestStatus>(req.Status, out var status);

                    if (category != null && client != null)
                    {
                        requests.Add(new ServiceRequest
                        {
                            CategoryId = category.Id,
                            ClientId = client.Id,
                            Title = req.Title,
                            Description = req.Description,
                            Address = req.Address,
                            Status = status,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
                if (requests.Any())
                {
                    await context.ServiceRequests.AddRangeAsync(requests);
                    await context.SaveChangesAsync();
                }
            }

            // 5. Seed Jobs
            var jobs = new List<Job>();
            if (seedData.Jobs != null && seedData.Jobs.Any())
            {
                foreach (var job in seedData.Jobs)
                {
                    var request = await context.ServiceRequests.FirstOrDefaultAsync(r => r.Title == job.ServiceRequestTitle);
                    var artisanUser = await context.Users.FirstOrDefaultAsync(u => u.Email == job.ArtisanEmail);
                    var clientUser = await context.Users.FirstOrDefaultAsync(u => u.Email == job.ClientEmail);

                    if (request != null && artisanUser != null && clientUser != null)
                    {
                        var artisanProfile = await context.ArtisanProfiles.FirstOrDefaultAsync(p => p.UserId == artisanUser.Id);

                        // Parse Enum
                        Enum.TryParse<JobStatus>(job.Status, out var jobStatus);

                        if (artisanProfile != null)
                        {
                            jobs.Add(new Job
                            {
                                ServiceRequestId = request.Id,
                                ArtisanId = artisanProfile.Id,
                                ClientId = clientUser.Id,
                                Status = jobStatus,
                                StartedAt = DateTime.UtcNow,
                                CompletedAt = jobStatus == JobStatus.Completed ? DateTime.UtcNow : null,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }
                if (jobs.Any())
                {
                    await context.Jobs.AddRangeAsync(jobs);
                    await context.SaveChangesAsync();
                }
            }
        }
    }

    // Helper models for deserialization
    public class SeedDataModel
    {
        public List<CategorySeed>? Categories { get; set; }
        public List<UserSeed>? Users { get; set; }
        public List<ArtisanProfileSeed>? ArtisanProfiles { get; set; }
        public List<ServiceRequestSeed>? ServiceRequests { get; set; }
        public List<JobSeed>? Jobs { get; set; }
    }

    public class CategorySeed
    {
        public string Name { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UserSeed
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }

    public class ArtisanProfileSeed
    {
        public string UserEmail { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public float Rating { get; set; }
        public int TotalReviews { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsVerified { get; set; }
    }

    public class ServiceRequestSeed
    {
        public string CategoryName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class JobSeed
    {
        public string ServiceRequestTitle { get; set; } = string.Empty;
        public string ArtisanEmail { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
