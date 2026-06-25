using System.Text;
using Herfa_back.Data;
using Herfa_back.Helpers;
using Herfa_back.Interfaces.IRepository;
using Herfa_back.Interfaces.IService;
using Herfa_back.Middleware;
using Herfa_back.Repositories;
using Herfa_back.Services;
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
        public static void Main(string[] args)
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

           

            // ================= SERVICES =================
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<JwtHelper>();


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
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();


            var app = builder.Build();

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

            // ================= RUN =================

            app.Run();
        }
    }
}
