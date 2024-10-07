using Business.Repositories.Classes;
using Business.Repositories.Interfaces;
using Business.Services.Interfaces;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Transactions.Intarfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IBookRepository Books { get; } 
        IAuthorRepository Authors { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
