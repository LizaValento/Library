using Business.Repositories.Interfaces;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories.Classes
{
    public class BookRepository : IBookRepository
    {
        private readonly Context _context;

        public BookRepository(Context context)
        {
            _context = context;
        }

        public Book GetById(int id)
        {
            return _context.Books
                .Include(b => b.Author)
                .FirstOrDefault(c => c.Id == id);
        }

        public void Add(Book Book)
        {
            _context.Books.Add(Book);
        }

        public IEnumerable<Book> GetAll()
        {
            return _context.Books
                .Include(b => b.Author)
                .ToList();
        }

        public IEnumerable<Book> GetFreeBooks() 
        {
            return _context.Books
                .Include(b => b.Author)
                .Where(b => b.UserId == null)
                .ToList();
        }

        public void Update(Book Book)
        {
            _context.Books.Update(Book);
        }

        public void Remove(Book Book)
        {
            _context.Books.Remove(Book);
        }

        public IEnumerable<Book> GetBooksByAuthorId(int authorId)
        {
            return _context.Books
                .Where(b => b.AuthorId == authorId)
                .ToList();
        }

        public IEnumerable<Book> GetBooksByGenre(string genre)
        {
            return _context.Books
                .Where(b => b.Genre == genre)
                .Include(b => b.Author)
                .ToList(); 
        }

        public IEnumerable<Book> GetAllWithTitles(string title)
        {
            return _context.Books
                .Where(b => b.Name == title)
                .Include(b => b.Author)
                .ToList();
        }

        public Book GetByISBN(string isbn)
        {
            return _context.Books
                .Include(b => b.Author)
                .FirstOrDefault(c => c.ISBN == isbn);
        }

        public IEnumerable<Book> GetBooksByAuthorNameAndLastName(string firstName, string lastName)
        {
            return _context.Books
               .Include(b => b.Author) 
               .Where(b => b.Author.FirstName == firstName && b.Author.LastName == lastName)
               .ToList();
        }
    }
}
