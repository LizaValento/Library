using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Entities;
using System.Xml.Linq;

namespace Business.Models
{
    public class BooksViewModel
    {
        public List<BookModel> Books { get; set; }
        public BookModel? Book { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
