using Business.Repositories.Interfaces;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.Classes
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly Context _context;

        public RefreshTokenRepository(Context context)
        {
            _context = context;
        }

        public RefreshToken GetByToken(string token)
        {
            return _context.RefreshTokens.FirstOrDefault(rt => rt.Token == token);
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public IEnumerable<RefreshToken> GetAll()
        {
            return _context.RefreshTokens.ToList();
        }

        public async Task<IEnumerable<RefreshToken>> GetAllAsync()
        {
            return await _context.RefreshTokens.ToListAsync();
        }

        public void Add(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public void Update(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            _context.SaveChanges();
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public void Delete(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }

        public RefreshToken GetByUserId(int userId)
        {
            return _context.RefreshTokens.FirstOrDefault(rt => rt.UserId == userId);
        }

        public async Task<RefreshToken> GetByUserIdAsync(int userId)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
        }
    }
}
