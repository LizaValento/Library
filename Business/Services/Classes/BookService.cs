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
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public BookModel GetBookById(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            var Book = _unitOfWork.Books.GetById(id.Value);
            return Book == null ? null : _mapper.Map<BookModel>(Book);
        }

        public List<BookModel> GetBooks()
        {
            var Books = _unitOfWork.Books.GetAll(); 
            return _mapper.Map<List<BookModel>>(Books);
        }

        public (List<BookModel> Books, int TotalCount) GetFreeBooks(int page, int pageSize)
        {
            var books = _unitOfWork.Books.GetFreeBooks();
            int totalCount = books.Count();

            var paginatedBooks = books
                .Skip((page - 1) * pageSize)
                .Take(pageSize) 
                .ToList(); 

            return (Books: _mapper.Map<List<BookModel>>(paginatedBooks), TotalCount: totalCount);
        }

        public void AddBook(BookModel bookModel)
        {
            if (bookModel == null)
            {
                throw new ArgumentNullException(nameof(bookModel), "Book model cannot be null.");
            }

            int authorId = _unitOfWork.Authors.GetOrCreateAuthorAsync(bookModel.AuthorName, bookModel.AuthorLastName).Result;

            var bookEntity = _mapper.Map<Book>(bookModel);
            bookEntity.AuthorId = authorId; 

            _unitOfWork.Books.Add(bookEntity);
            _unitOfWork.Complete();
        }

        public void UpdateBook(BookModel bookModel)
        {
            if (bookModel == null)
            {
                throw new ArgumentNullException(nameof(bookModel), "Модель книги не может быть пустой.");
            }

            var existingBook = _unitOfWork.Books.GetById(bookModel.Id);
            if (existingBook == null)
            {
                throw new InvalidOperationException("Книга не найдена.");
            }

            existingBook.Name = bookModel.Name;
            existingBook.AuthorId = _unitOfWork.Authors.GetOrCreateAuthorAsync(bookModel.AuthorName, bookModel.AuthorLastName).Result;
            existingBook.BookImage = bookModel.BookImage;
            existingBook.ReturnDate = bookModel.ReturnDate;
            existingBook.IssueDate = bookModel.IssueDate;
            existingBook.Description = bookModel.Description;
            existingBook.ISBN = bookModel.ISBN;
            existingBook.Genre = bookModel.Genre;

            _unitOfWork.Books.Update(existingBook);
            _unitOfWork.Complete();
        }
        public void DeleteBook(int id)
        {
            var book = _unitOfWork.Books.GetById(id);
            
            _unitOfWork.Books.Remove(book);
            _unitOfWork.Complete(); 
        }

        public void TakeBook(BookModel bookModel)
        {
            if (bookModel == null)
            {
                throw new ArgumentNullException(nameof(bookModel), "Book model cannot be null.");
            }

            var existingBook = _unitOfWork.Books.GetById(bookModel.Id);
            if (existingBook == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            _mapper.Map(bookModel, existingBook);

            _unitOfWork.Books.Update(existingBook);

            _unitOfWork.Complete();
        }

        public List<BookModel> GetAllBooksByGenres(string genre)
        {
            var Books = _unitOfWork.Books.GetBooksByGenre(genre).ToList();
            return _mapper.Map<List<BookModel>>(Books);
        }

        public List<BookModel> GetAllBooksByAuthor(string firstName, string lastName)
        {
            var Books = _unitOfWork.Books.GetBooksByAuthorNameAndLastName(firstName, lastName).ToList();
            return _mapper.Map<List<BookModel>>(Books);
        }

        public List<BookModel> SearchBooksByTitle(string title)
        {
            var Books = _unitOfWork.Books.GetAllWithTitles(title);
            return _mapper.Map<List<BookModel>>(Books);
        }

        public BookModel SearchBookByISBN(string isbn)
        {
            var Book = _unitOfWork.Books.GetByISBN(isbn);
            return _mapper.Map<BookModel>(Book);
        }
    }
}
