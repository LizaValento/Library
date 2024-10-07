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

namespace Business.Services.Classes
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
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

        public List<UserModel> GetUsers()
        {
            var users = _unitOfWork.Users.GetAll();
            return _mapper.Map<List<UserModel>>(users);
        }

        public void AddUser(UserModel userModel)
        {
            if (userModel == null)
            {
                throw new ArgumentNullException(nameof(userModel), "User model cannot be null.");
            }

            var userEntity = _mapper.Map<User>(userModel);
            _unitOfWork.Users.Add(userEntity);
            _unitOfWork.Complete();
        }

        public void UpdateUser(UserModel userModel)
        {
            if (userModel == null)
            {
                throw new ArgumentNullException(nameof(userModel), "User model cannot be null.");
            }

            var userEntity = _mapper.Map<User>(userModel);
            _unitOfWork.Users.Update(userEntity);
            _unitOfWork.Complete();
        }

        public void DeleteUser(int id)
        {
            var user = _unitOfWork.Users.GetById(id);
            if (user != null)
            {
                _unitOfWork.Users.Remove(user);
                _unitOfWork.Complete();
            }
        }

        public void RegisterUser(RegisterModel userModel)
        {
            if (userModel == null)
            {
                throw new ArgumentNullException(nameof(userModel), "Register model cannot be null.");
            }

            var existingUser = _unitOfWork.Users.GetByNickname(userModel.Nickname);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists.");
            }


            var userEntity = new User
            {
                FirstName = userModel.FirstName,
                Password = userModel.Password,
                LastName = userModel.LastName,
                Nickname = userModel.Nickname,
                Role = "User"
            };

            _unitOfWork.Users.Add(userEntity);
            _unitOfWork.Complete();
        }

        public TokenModel Authenticate(LoginModel model)
        {
            var user = _unitOfWork.Users.GetByNickname(model.Nickname);
            if (user == null || user.Password != model.Password)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nickname),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            _tokenService.SaveRefreshToken(user.Id, refreshToken);

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public (List<BookModel> Books, int TotalCount) GetUserBooks(int userId, int page, int pageSize)
        {
            var (books, totalCount) = _unitOfWork.Users.GetUserBooks(userId, page, pageSize);
            var bookModels = _mapper.Map<List<BookModel>>(books); 

            return (bookModels, totalCount);
        }
    }
}
