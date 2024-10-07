
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
using Microsoft.Extensions.DependencyInjection;

namespace Business.Services.Classes
{
    public class BookReturnHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public BookReturnHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var bookReturnService = scope.ServiceProvider.GetRequiredService<IBookReturnService>();
                bookReturnService.CheckReturnDates(); 
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}