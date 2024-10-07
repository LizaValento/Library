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
    public interface IUserService
    {
        UserModel GetUserById(int? id);
        List<UserModel> GetUsers();
        void AddUser(UserModel user);
        void UpdateUser(UserModel user);
        void DeleteUser(int id);
        void RegisterUser(RegisterModel userModel);
        TokenModel Authenticate(LoginModel model);
        (List<BookModel> Books, int TotalCount) GetUserBooks(int userId, int page, int pageSize);
    }
}
