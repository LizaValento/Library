using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Book GetById(int id);
        IEnumerable<Book> GetAll(); 
        void Add(Book Book);
        void Update(Book Book);
        void Remove(Book Book);
        IEnumerable<Book> GetBooksByAuthorId(int authorId);
        IEnumerable<Book> GetFreeBooks();
        IEnumerable<Book> GetBooksByGenre(string genre);
        IEnumerable<Book> GetAllWithTitles(string title);
        Book GetByISBN(string isbn);
        IEnumerable<Book> GetBooksByAuthorNameAndLastName(string firstName, string lastName);
    }
}
