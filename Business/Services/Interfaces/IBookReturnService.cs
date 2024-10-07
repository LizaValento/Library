using Business.Models;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Interfaces
{
    public interface IBookReturnService
    {
        void CheckReturnDates();
    }
}
