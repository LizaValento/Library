using Business.Models;
using Data.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Services.Interfaces
{
    public interface ITokenService
    {
        // Генерация токенов
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();

        // Асинхронные методы
        Task SaveRefreshTokenAsync(int userId, string token);
        Task<RefreshTokenModel> GetRefreshTokenAsync(string token);
        Task<bool> ValidateRefreshTokenAsync(int userId, string token);
        Task CheckAndUpdateTokensAsync();
        Task<TokenModel> RefreshTokenAsync(string refreshToken);
        Task<IEnumerable<RefreshToken>> GetAllAsync();

        // Синхронные методы
        void SaveRefreshToken(int userId, string token);
        RefreshTokenModel GetRefreshToken(string token);
        bool ValidateRefreshToken(int userId, string token);
        void CheckAndUpdateTokens();
        TokenModel RefreshToken(string refreshToken);
        IEnumerable<RefreshToken> GetAll();

        UserModel GetUserById(int? id);
    }
}
