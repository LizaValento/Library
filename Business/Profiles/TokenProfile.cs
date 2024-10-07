using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Models;
using Data.Entities;

namespace Business.Profiles
{
    public class TokenProfile : Profile
    {
        public TokenProfile()
        {
            CreateMap<RefreshTokenModel, RefreshToken>(); 
            CreateMap<RefreshToken, RefreshTokenModel>();
        }
    }
}
