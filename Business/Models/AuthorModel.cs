using System;
using System.Collections.Generic;
using System.ComponentModel;
using Data.Entities;

namespace Business.Models
{
    public class AuthorModel
    {
        public AuthorModel() { }
        public AuthorModel(Author Author)
        {
            Id = Author.Id;
            FirstName = Author.FirstName;
            LastName = Author.LastName;
            DateOfBirth = Author.DateOfBirth?.ToString("yyyy-MM-dd") ?? "Не указано"; 
            Country = Author?.Country ?? "Не указано";
            Books = Author.Books?.Select(b => new BookModel(b)).ToList() ?? new List<BookModel>();
        }
        public int Id { get; set; }
        [DisplayName("Имя:")]
        public string FirstName { get; set; }
        [DisplayName("Фамилия:")]
        public string LastName { get; set; }
        [DisplayName("Никнейм:")]
        public string? DateOfBirth { get; set; }
        public string? Country { get; set; }
        public List<BookModel> Books { get; set; }
    }
}
