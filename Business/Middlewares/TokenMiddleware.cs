using Business.Models;
using Business.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.ExceptionHandler
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JWTSettings _jwtSettings;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TokenMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory, IOptions<JWTSettings> jwtSettings)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies["AccessToken"];
            var refreshToken = context.Request.Cookies["RefreshToken"];

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

                // Проверяем наличие refresh token
                if (!string.IsNullOrEmpty(refreshToken) && await ValidateRefreshTokenAsync(tokenService, refreshToken))
                {
                    // Проверяем наличие access token
                    if (string.IsNullOrEmpty(token) || !ValidateAccessToken(token))
                    {
                        var userId = await GetUserIdFromRefreshTokenAsync(tokenService, refreshToken);
                        if (userId != null)
                        {
                            var user = tokenService.GetUserById(userId.Value);
                            var claims = new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(ClaimTypes.Name, user.Nickname),
                                new Claim(ClaimTypes.Role, user.Role)
                            };

                            var newAccessToken = tokenService.GenerateAccessToken(claims);

                            var claimsIdentity = new ClaimsIdentity(claims, "Custom");
                            context.User = new ClaimsPrincipal(claimsIdentity);

                            // Сохраняем новый access token в куки
                            context.Response.Cookies.Append("AccessToken", newAccessToken, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict
                            });
                        }
                    }
                }
            }

            await _next(context);
        }

        private bool ValidateAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> ValidateRefreshTokenAsync(ITokenService tokenService, string refreshToken)
        {
            var refreshTokenEntity = await tokenService.GetRefreshTokenAsync(refreshToken);
            return refreshTokenEntity != null && refreshTokenEntity.ExpiresAt > DateTime.UtcNow;
        }

        private async Task<int?> GetUserIdFromRefreshTokenAsync(ITokenService tokenService, string refreshToken)
        {
            var refreshTokenEntity = await tokenService.GetRefreshTokenAsync(refreshToken);
            return refreshTokenEntity?.UserId;
        }
    }
}

