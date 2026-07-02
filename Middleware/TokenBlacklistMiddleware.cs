using System.IdentityModel.Tokens.Jwt;
using Herfa_back.Data;
using Microsoft.EntityFrameworkCore;

namespace Herfa_back.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly string[] _excludedPaths =
        {
            "/api/auth/login",
            "/api/auth/register",
            "/api/auth/forgot-password",
            "/api/auth/reset-password",
            "/api/auth/refresh"
        };

        public TokenBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();

            if (path is not null && _excludedPaths.Any(p => path.Contains(p)))
            {
                await _next(context);
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader is not null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader["Bearer ".Length..].Trim();

                if (!string.IsNullOrWhiteSpace(token))
                {
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(token);
                        var jti = jwtToken.Claims
                            .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                        if (!string.IsNullOrWhiteSpace(jti))
                        {
                            var isBlacklisted = await dbContext.BlacklistedTokens
                                .AnyAsync(t => t.Jti == jti);

                            if (isBlacklisted)
                            {
                                context.Response.StatusCode = 401;
                                await context.Response.WriteAsJsonAsync(new
                                {
                                    message = "Token has been revoked"
                                });
                                return;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            await _next(context);
        }
    }
}
