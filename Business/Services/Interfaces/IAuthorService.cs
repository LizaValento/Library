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
    public interface IAuthorService
    {
        AuthorModel GetAuthorByIdForBooks(int? id, int page, int pageSize, out int totalBooks);
        AuthorModel GetAuthorById(int? id);
        (List<AuthorModel> Authors, int TotalCount) GetAuthorsPagination(int page, int pageSize);
        void AddAuthor(AuthorModel Author);
        void UpdateAuthor(AuthorModel Author);
        void DeleteAuthor(int id);
        Task<int> GetOrCreateAuthorAsync(string firstName, string lastName);
        List<AuthorModel> GetAuthors();
    }
}
