using Business.Models;
using Business.Services.Interfaces;
using Data.Entities;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Business.Transactions.Intarfaces;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Business.Services.Classes
{
    public class BookReturnService : BackgroundService, IBookReturnService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger<BookReturnService> _logger;

        public BookReturnService(IUnitOfWorkFactory unitOfWorkFactory, ILogger<BookReturnService> logger)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                CheckReturnDates(); 
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        public void CheckReturnDates()
        {
            using var unitOfWork = _unitOfWorkFactory.Create(); 
            var overdueBooks = unitOfWork.Books.GetAll()
                .Where(b => b.ReturnDate < DateTime.Now && b.UserId != null)
                .ToList();

            foreach (var book in overdueBooks)
            {
                book.UserId = null; 
                unitOfWork.Books.Update(book); 
            }

            unitOfWork.Complete(); 
            _logger.LogInformation($"{overdueBooks.Count} books returned.");
        }
    }


}
