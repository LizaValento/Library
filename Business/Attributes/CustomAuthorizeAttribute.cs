using Business.Models;
using Business.Services.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class CustomAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly ITokenService _tokenService;
    private readonly JWTSettings _jwtSettings;

    public CustomAuthorizeAttribute(ITokenService tokenService, IOptions<JWTSettings> jwtSettings)
    {
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value; 
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Извлекаем токены из куков
        var token = context.HttpContext.Request.Cookies["AccessToken"];
        var refreshToken = context.HttpContext.Request.Cookies["RefreshToken"];

        // Проверяем наличие refresh token
        if (string.IsNullOrEmpty(refreshToken) || !await ValidateRefreshTokenAsync(refreshToken))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Проверяем наличие access token
        if (string.IsNullOrEmpty(token) || !ValidateAccessToken(token))
        {
            // Если access token не валиден, пытаемся обновить его
            var userId = await GetUserIdFromRefreshTokenAsync(refreshToken);
            if (userId == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var user = _tokenService.GetUserById(userId);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nickname),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var newAccessToken = _tokenService.GenerateAccessToken(claims); // Генерируем новый access token на основе userId

            // Сохраняем новый access token в куки, refresh token остаётся прежним
            context.HttpContext.Response.Cookies.Append("AccessToken", newAccessToken);
        }
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

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
    {
        var refreshTokenEntity = await _tokenService.GetRefreshTokenAsync(refreshToken);
        return refreshTokenEntity != null && refreshTokenEntity.ExpiresAt > DateTime.UtcNow;
    }

    private async Task<int?> GetUserIdFromRefreshTokenAsync(string refreshToken)
    {
        var refreshTokenEntity = await _tokenService.GetRefreshTokenAsync(refreshToken);
        return refreshTokenEntity?.UserId;
    }
}
