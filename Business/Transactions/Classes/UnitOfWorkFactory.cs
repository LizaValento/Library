using Business.Repositories.Classes;
using Business.Repositories.Interfaces;
using Business.Services.Classes;
using Business.Services.Interfaces;
using Business.Transactions.Intarfaces;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Transactions.Classes
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWorkFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IUnitOfWork Create()
        {
            var context = _serviceProvider.GetRequiredService<Context>();
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var bookRepository = _serviceProvider.GetRequiredService<IBookRepository>();
            var authorRepository = _serviceProvider.GetRequiredService<IAuthorRepository>();
            var refreshTokenRepository = _serviceProvider.GetRequiredService<IRefreshTokenRepository>();

            return new UnitOfWork(context, userRepository, bookRepository, authorRepository, refreshTokenRepository);
        }
    }

}
