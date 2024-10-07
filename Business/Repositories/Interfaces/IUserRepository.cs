using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User GetById(int id);
        User GetByNickname(string nickname);
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Remove(User user);
        Task<User> GetByIdAsync(int id);
        (List<Book> Books, int TotalCount) GetUserBooks(int userId, int page, int pageSize);
    }
}
