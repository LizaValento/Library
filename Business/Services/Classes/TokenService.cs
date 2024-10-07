using AutoMapper;
using Business.Models;
using Business.Services.Interfaces;
using Business.Transactions.Intarfaces;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Classes
{
    public class TokenService : ITokenService
    {
        private readonly JWTSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor, IOptions<JWTSettings> jwtSettings, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            if (claims == null || !claims.Any())
            {
                throw new ArgumentNullException(nameof(claims), "Claims не могут быть null или пустыми.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenLifetime),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task SaveRefreshTokenAsync(int userId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token не может быть null или пустым.");
            }

            var refreshTokenEntity = await _unitOfWork.RefreshTokens.GetByUserIdAsync(userId); 
            if (refreshTokenEntity != null)
            {
                refreshTokenEntity.Token = token;
                refreshTokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(30); 
                _unitOfWork.RefreshTokens.Update(refreshTokenEntity);
            }
            else
            {
                var refreshToken = new RefreshTokenModel
                {
                    UserId = userId,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddDays(30) 
                };

                var refreshTokenEntityNew = _mapper.Map<RefreshToken>(refreshToken);
                _unitOfWork.RefreshTokens.Add(refreshTokenEntityNew); 
            }

            await _unitOfWork.CompleteAsync();
        }

        public void SaveRefreshToken(int userId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token не может быть null или пустым.");
            }

            var refreshTokenEntity = _unitOfWork.RefreshTokens.GetByUserId(userId);

            if (refreshTokenEntity != null)
            {
                refreshTokenEntity.Token = token;
                refreshTokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(30);
                _unitOfWork.RefreshTokens.Update(refreshTokenEntity); 
            }
            else
            {
                var refreshToken = new RefreshTokenModel
                {
                    UserId = userId,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddDays(30) 
                };

                var refreshTokenEntityNew = _mapper.Map<RefreshToken>(refreshToken);
                _unitOfWork.RefreshTokens.Add(refreshTokenEntityNew);
            }

            _unitOfWork.Complete();
        }

        public async Task<RefreshTokenModel> GetRefreshTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token не может быть null или пустым.");
            }

            var refreshTokenEntity = await _unitOfWork.RefreshTokens.GetByTokenAsync(token);
            if (refreshTokenEntity == null)
            {
                return null;
            }

            return _mapper.Map<RefreshTokenModel>(refreshTokenEntity);
        }

        public RefreshTokenModel GetRefreshToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token не может быть null или пустым.");
            }

            var refreshTokenEntity = _unitOfWork.RefreshTokens.GetByToken(token);
            if (refreshTokenEntity == null)
            {
                return null;
            }

            return _mapper.Map<RefreshTokenModel>(refreshTokenEntity);
        }

        public async Task<bool> ValidateRefreshTokenAsync(int userId, string token)
        {
            var refreshToken = await GetRefreshTokenAsync(token);
            return refreshToken != null && refreshToken.UserId == userId && refreshToken.ExpiresAt > DateTime.UtcNow;
        }

        public bool ValidateRefreshToken(int userId, string token)
        {
            var refreshToken = GetRefreshToken(token);
            return refreshToken != null && refreshToken.UserId == userId && refreshToken.ExpiresAt > DateTime.UtcNow;
        }

        public async Task CheckAndUpdateTokensAsync()
        {
            var refreshTokenEntities = await _unitOfWork.RefreshTokens.GetAllAsync(); 
            var refreshTokens = _mapper.Map<List<RefreshTokenModel>>(refreshTokenEntities);

            foreach (var refreshToken in refreshTokens)
            {
                if (refreshToken.ExpiresAt < DateTime.UtcNow)
                {
                    var refreshTokensModel = _mapper.Map<RefreshToken>(refreshToken);
                    _unitOfWork.RefreshTokens.Delete(refreshTokensModel);
                }
            }

            await _unitOfWork.CompleteAsync(); 
        }

        public void CheckAndUpdateTokens()
        {
            var refreshTokenEntities = _unitOfWork.RefreshTokens.GetAll();
            var refreshTokens = _mapper.Map<List<RefreshTokenModel>>(refreshTokenEntities);

            foreach (var refreshToken in refreshTokens)
            {
                if (refreshToken.ExpiresAt < DateTime.UtcNow)
                {
                    var refreshTokensModel = _mapper.Map<RefreshToken>(refreshToken);
                    _unitOfWork.RefreshTokens.Delete(refreshTokensModel);
                }
            }

            _unitOfWork.Complete(); 
        }

        public async Task<TokenModel> RefreshTokenAsync(string refreshToken)
        {
            var refreshTokenEntity = await GetRefreshTokenAsync(refreshToken);
            if (refreshTokenEntity == null || refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(refreshTokenEntity.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nickname),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var newAccessToken = GenerateAccessToken(claims);
            var newRefreshToken = GenerateRefreshToken();

            await SaveRefreshTokenAsync(user.Id, newRefreshToken);

            return new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public TokenModel RefreshToken(string refreshToken)
        {
            var refreshTokenEntity = GetRefreshToken(refreshToken);
            if (refreshTokenEntity == null || refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var user = _unitOfWork.Users.GetById(refreshTokenEntity.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nickname),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var newAccessToken = GenerateAccessToken(claims);
            var newRefreshToken = GenerateRefreshToken();

            SaveRefreshTokenAsync(user.Id, newRefreshToken);

            return new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
        public async Task<IEnumerable<RefreshToken>> GetAllAsync()
        {
            var refreshTokenEntities = await _unitOfWork.RefreshTokens.GetAllAsync();
            return refreshTokenEntities;
        }

        public IEnumerable<RefreshToken> GetAll()
        {
            var refreshTokenEntities = _unitOfWork.RefreshTokens.GetAll();
            return refreshTokenEntities;
        }

        public UserModel GetUserById(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            var user = _unitOfWork.Users.GetById(id.Value);
            return user == null ? null : _mapper.Map<UserModel>(user);
        }
    }
}
