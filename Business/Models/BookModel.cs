using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Entities;
using System.Xml.Linq;

namespace Business.Models
{
    public class BookModel
    {
        public BookModel() { }
        public BookModel(Book Book)
        {
            AuthorName = Book.Author.FirstName;
            AuthorLastName = Book.Author.LastName;
            Id = Book.Id;
            Name = Book.Name;
            ISBN = Book.ISBN;
            Genre = Book.Genre;
            Description = Book.Description;
            AuthorId = Book.AuthorId;
            UserId = Book?.UserId;
            IssueDate = Book.IssueDate;
            ReturnDate = Book.ReturnDate;
            BookImage = Book?.BookImage;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ISBN { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public int? AuthorId { get; set; }
        public int? UserId { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? BookImage { get; set; }
        public string AuthorName { get; set; }
        public string AuthorLastName { get; set; }
    }
}
