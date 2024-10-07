using Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        RefreshToken GetByToken(string token);
        void Add(RefreshToken refreshToken);
        void Update(RefreshToken refreshToken);
        void Delete(RefreshToken refreshToken);
        IEnumerable<RefreshToken> GetAll();
        RefreshToken GetByUserId(int userId);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task DeleteAsync(RefreshToken refreshToken);
        Task<IEnumerable<RefreshToken>> GetAllAsync();
        Task<RefreshToken> GetByUserIdAsync(int userId);
    }
}
