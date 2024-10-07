using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        Author GetById(int id);
        IEnumerable<Author> GetAll();
        void Add(Author Author);
        void Update(Author Author);
        void Remove(Author Author);
        Task<int> GetOrCreateAuthorAsync(string firstName, string lastName);
    }
}
