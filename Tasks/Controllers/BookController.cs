using Business.Models;
using Business.Services.Classes;
using Business.Services.Interfaces;
using Business.Validators;
using Data.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Tasks.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _BookService; 
        private readonly IValidator<BookModel> _bookValidator;
        private readonly IAuthorService _AuthorService;

        public BookController(IBookService BookService, IValidator<BookModel> bookValidator, IAuthorService AuthorService)
        {
            _BookService = BookService;
            _bookValidator = bookValidator;
            _AuthorService = AuthorService;
        }

        public ViewResult Main(int page = 1, int pageSize = 5)
        {
            var (books, totalCount) = _BookService.GetFreeBooks(page, pageSize);

            var model = new BooksViewModel
            {
                Books = books,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddBook(BookModel model, IFormFile BookImage)
        {
            var validationResult = _bookValidator.Validate(model);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ModelState); 
            }
            else
            {
                if (BookImage != null && BookImage.Length > 0)
                {
                    var fileName = Path.GetFileName(BookImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await BookImage.CopyToAsync(stream);
                    }

                    model.BookImage = $"/images/{fileName}";
                }
                else
                {
                    model.BookImage = "/images/DefaultBook.jpg";
                }

                model.IssueDate = DateTime.Now;
                model.ReturnDate = DateTime.Now;

                _BookService.AddBook(model);

                return RedirectToAction("Main", "Book");
            }
        }

        [HttpGet("Book/AddBook")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult AddBook()
        {
            return View();
        }

        [HttpGet("Book/ViewBook/{BookId}")]
        public ActionResult ViewBook(int? BookId)
        {
            var Book = _BookService.GetBookById(BookId);
            return View(Book);
        }


        [HttpPost]
        public async Task<ActionResult> UpdateBookAsync(BookModel model, IFormFile BookImage)
        {
            var validationResult = _bookValidator.Validate(model);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ModelState);
            }
            else
            {
                if (BookImage != null && BookImage.Length > 0)
                {
                    var fileName = Path.GetFileName(BookImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await BookImage.CopyToAsync(stream);
                    }

                    model.BookImage = $"/images/{fileName}";
                }
                else
                {
                    model.BookImage = "/images/DefaultBook.jpg";
                }
                var existingBook = _BookService.GetBookById(model.Id);

                if (existingBook == null)
                {
                    return NotFound();
                }
                model.UserId = existingBook.UserId;
                model.IssueDate = DateTime.Now;
                model.ReturnDate = DateTime.Now;
                _BookService.UpdateBook(model);
                return RedirectToAction("ViewBook", "Book", new { id = model.Id });
            }
        }

        [HttpGet("Book/UpdateBook/{BookId}")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult UpdateBook(int BookId)
        {
            var Book = _BookService.GetBookById(BookId);
            return View(Book);
        }

        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult DeleteBook(int Id)
        {
            _BookService.DeleteBook(Id);
            return RedirectToAction("Main", "Book");
        }

        [HttpPost]
        public ActionResult TakeBook(int Id)
        {    
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var book = _BookService.GetBookById(Id);
            if (book == null)
            {
                return NotFound(); 
            }

            book.UserId = userId; 
            book.IssueDate = DateTime.Now; 
            book.ReturnDate = DateTime.Now.AddDays(7);

            _BookService.TakeBook(book); 

            return RedirectToAction("ViewBook", "Book", new { id = book.Id });
        }

        [HttpGet("Book/Search")]
        public IActionResult Search(int page = 1, int pageSize = 5)
        {
            var (books, totalCount) = _BookService.GetFreeBooks(page, pageSize);
            var model = new BooksViewModel
            {
                Books = books,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(model);
        }

        [HttpPost("Book/SearchByTitle")]
        public IActionResult SearchByTitle(BookModel model)
        {
            var books = _BookService.SearchBooksByTitle(model.Name);

            TempData["BookViewModel"] = JsonConvert.SerializeObject(books);

            return RedirectToAction("SearchResults");
        }

        [HttpGet("Book/SearchResults")]
        public ActionResult SearchResults(int page = 1, int pageSize = 5)
        {
            var booksJson = HttpContext.Session.GetString("FilteredBooks");

            List<BookModel> foundBooks;

            if (!string.IsNullOrEmpty(booksJson))
            {
                foundBooks = JsonConvert.DeserializeObject<List<BookModel>>(booksJson);
            }
            else
            {
                return View(new BooksViewModel());
            }

            var totalCount = foundBooks.Count;
            var pagedBooks = foundBooks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var resultModel = new BooksViewModel
            {
                Books = pagedBooks,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(resultModel);
        }

        [HttpPost("Book/SearchByISBN")]
        public IActionResult SearchByISBN(BookModel model)
        {
            var book = _BookService.SearchBookByISBN(model.ISBN);
            return RedirectToAction("ViewBook", "Book", new { BookId = book.Id });
        }

        [HttpPost("Book/GenreFilter")]
        public IActionResult GenreFilter(BookModel model)
        {
            var books = _BookService.GetAllBooksByGenres(model.Genre);

            HttpContext.Session.SetString("FilteredBooks", JsonConvert.SerializeObject(books));

            return RedirectToAction("SearchResults");
        }


        [HttpPost("Book/AuthorFilter")]
        public IActionResult AuthorFilter(BookModel model)
        {
            var books = _BookService.GetAllBooksByAuthor(model.AuthorName,model.AuthorLastName);
            TempData["FilteredBooks"] = JsonConvert.SerializeObject(books);

            return RedirectToAction("SearchResults");
        }
    }
}
