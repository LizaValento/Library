using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Entities;
using System.Xml.Linq;

namespace Business.Models
{
    public class AuthorViewModel
    {
        public List<AuthorModel> Authors { get; set; }
        public AuthorModel Author { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
