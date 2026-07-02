using System.Text;
using Herfa_back.Data;
using Herfa_back.Helpers;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Interfaces.IService;
using Herfa_back.Middleware;
using Herfa_back.Repositories;
using Herfa_back.Services;
using Herfa_back.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;


namespace Herfa_back
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ================= DATABASE =================
            builder.Services.AddDbContext<AppDbContext>(options =>
                 options.UseSqlServer(
                 builder.Configuration.GetConnectionString("DefaultConnection"),
                 sqlServerOptionsAction: sqlOptions =>
                 {
                      sqlOptions.EnableRetryOnFailure(
                      maxRetryCount: 5,           
                      maxRetryDelay: TimeSpan.FromSeconds(30), 
                      errorNumbersToAdd: null);
                 }
            ));

           

            // ================= DEPENDENCY INJECTION =================

            builder.Services.AddScoped<JwtHelper>();

            // Repositories (interface-based)
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IArtisanRepository, ArtisanRepository>();
            builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IServiceOfferRepository, ServiceOfferRepository>();

            // Repositories (concrete — needed by CategoryService, ArtisanService, JobService)
            builder.Services.AddScoped<CategoryRepository>();
            builder.Services.AddScoped<ArtisanRepository>();
            builder.Services.AddScoped<JobRepository>();

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJobService, JobService>();
            builder.Services.AddScoped<IClientService, ClientService>(); // Client Service
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IArtisanService, ArtisanService>();
            builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
            builder.Services.AddScoped<IServiceOfferService, ServiceOfferService>();

            // SignalR
            builder.Services.AddSignalR();

            // ================= JWT AUTH =================

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,

                        ValidateAudience = true,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,

                        ValidIssuer =
                            builder.Configuration["JWT:Issuer"],

                        ValidAudience =
                            builder.Configuration["JWT:Audience"],

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    builder.Configuration["JWT:Key"]!
                                )
                            )
                    };
                
                // SignalR JWT Support
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });


            // ================= AUTHORIZATION =================

            builder.Services.AddAuthorization();


            // ================= CONTROLLERS =================

            builder.Services.AddControllers();


            // ================= SWAGGER =================

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Herfa API",
                        Version = "v1",
                        Description = "Authentication API with JWT"
                    });

                // JWT Swagger Auth
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",

                        Type = SecuritySchemeType.Http,

                        Scheme = "bearer",

                        BearerFormat = "JWT",

                        In = ParameterLocation.Header,

                        Description =
                            "Enter JWT Token like this: Bearer {your token}"
                    });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
                    });
            });


            // ================= CORS =================

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:4200") // Update with actual frontend origins
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials(); // Required for SignalR
                    });
            });

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();


            var app = builder.Build();

            // ================= DATA SEEDING =================
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    await DataSeeder.SeedAsync(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI();
            }

            // ================= MIDDLEWARE =================

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();

            app.UseMiddleware<TokenBlacklistMiddleware>();

            app.UseAuthorization();

            // ================= MAP CONTROLLERS =================

            app.MapControllers();
            app.MapHub<NotificationHub>("/hubs/notifications");

            // ================= RUN =================

            app.Run();
        }
    }
}
