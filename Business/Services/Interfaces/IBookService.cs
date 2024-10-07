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
    public interface IBookService
    {
        BookModel GetBookById(int? id);
        List<BookModel> GetBooks();
        void AddBook(BookModel Book);
        void UpdateBook(BookModel Book);
        void DeleteBook(int id); 
        (List<BookModel> Books, int TotalCount) GetFreeBooks(int page, int pageSize);
        void TakeBook(BookModel BookModel);
        List<BookModel> GetAllBooksByGenres(string genre);
        List<BookModel> GetAllBooksByAuthor(string firstName, string lastName);
        BookModel SearchBookByISBN(string isbn);
        List<BookModel> SearchBooksByTitle(string title);
    }
}
