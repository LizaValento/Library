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
    public class AuthorRepository : IAuthorRepository
    {
        private readonly Context _context;

        public AuthorRepository(Context context)
        {
            _context = context;
        }

        public Author GetById(int id)
        {
            return _context.Authors
                .Include(x => x.Books)
                .FirstOrDefault(c => c.Id == id);
        }

        public void Add(Author Author)
        {
            _context.Authors.Add(Author);
        }

        public IEnumerable<Author> GetAll()
        {
            return _context.Authors.ToList();
        }

        public void Update(Author Author)
        {
            _context.Authors.Update(Author);
        }

        public void Remove(Author Author)
        {
            _context.Authors.Remove(Author);
        }

        public async Task<int> GetOrCreateAuthorAsync(string firstName, string lastName)
        {
            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.FirstName.ToLower() == firstName.ToLower() &&
                                           a.LastName.ToLower() == lastName.ToLower());

            if (author != null)
            {
                return author.Id;
            }

            author = new Author
            {
                FirstName = firstName,
                LastName = lastName,
                Country = "Не указано",
                DateOfBirth = new DateTime(2000, 1, 1)
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return author.Id; 
        }

    }
}

