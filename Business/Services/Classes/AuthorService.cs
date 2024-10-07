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
using Microsoft.Extensions.Hosting;

namespace Business.Services.Classes
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public AuthorModel GetAuthorByIdForBooks(int? id, int page, int pageSize, out int totalBooks)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            var author = _unitOfWork.Authors.GetById(id.Value);
            if (author == null)
            {
                totalBooks = 0;
                return null;
            }

            var booksQuery = _unitOfWork.Books.GetBooksByAuthorId(author.Id); 
            totalBooks = booksQuery.Count();

            var books = booksQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var authorModel = _mapper.Map<AuthorModel>(author);
            authorModel.Books = _mapper.Map<List<BookModel>>(books);
            return authorModel;
        }
        public AuthorModel GetAuthorById(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            var Author = _unitOfWork.Authors.GetById(id.Value);
            return Author == null ? null : _mapper.Map<AuthorModel>(Author);
        }

        public List<AuthorModel> GetAuthors()
        {
            var Authors = _unitOfWork.Authors.GetAll();
            return _mapper.Map<List<AuthorModel>>(Authors);
        }

        public (List<AuthorModel> Authors, int TotalCount) GetAuthorsPagination(int page, int pageSize)
        {
            var Authors = _unitOfWork.Authors.GetAll(); 
            int totalCount = Authors.Count();

            var paginatedAuthors = Authors
                .Skip((page - 1) * pageSize) 
                .Take(pageSize) 
                .ToList();

            return (Authors: _mapper.Map<List<AuthorModel>>(paginatedAuthors), TotalCount: totalCount);
        }

        public void AddAuthor(AuthorModel AuthorModel)
        {
            if (AuthorModel == null)
            {
                throw new ArgumentNullException(nameof(AuthorModel), "Author model cannot be null.");
            }

            var AuthorEntity = _mapper.Map<Author>(AuthorModel);
            if (AuthorEntity == null)
            {
                throw new InvalidOperationException("Mapped author entity cannot be null.");
            }
            _unitOfWork.Authors.Add(AuthorEntity);
            _unitOfWork.Complete(); 
        }

        public void UpdateAuthor(AuthorModel AuthorModel)
        {
            if (AuthorModel == null)
            {
                throw new ArgumentNullException(nameof(AuthorModel), "Author model cannot be null.");
            }

            var AuthorEntity = _mapper.Map<Author>(AuthorModel);
            _unitOfWork.Authors.Update(AuthorEntity);
            _unitOfWork.Complete(); 
        }

        public void DeleteAuthor(int id)
        {
            var author = _unitOfWork.Authors.GetById(id);
            if (author != null)
            { 
                var books = _unitOfWork.Books.GetBooksByAuthorId(id); 
                foreach (var book in books)
                {
                    _unitOfWork.Books.Remove(book);
                }
                 
                _unitOfWork.Authors.Remove(author);
                _unitOfWork.Complete(); 
            }
        }

        public async Task<int> GetOrCreateAuthorAsync(string firstName, string lastName)
        {
            return await _unitOfWork.Authors.GetOrCreateAuthorAsync(firstName, lastName);
        }
    }
}
