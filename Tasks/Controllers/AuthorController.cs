using Business.Models;
using Business.Services.Classes;
using Business.Services.Interfaces;
using Business.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace Tasks.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorService _AuthorService;
        private readonly IValidator<AuthorModel> _AuthorValidator;

        public AuthorController(IAuthorService AuthorService, IValidator<AuthorModel> AuthorValidator)
        {
            _AuthorService = AuthorService;
            _AuthorValidator = AuthorValidator;
        }

        [HttpGet("Author/Authors/")]
        public ViewResult Authors(int page = 1, int pageSize = 5)
        {
            var (Authors, totalCount) = _AuthorService.GetAuthorsPagination(page, pageSize);

            var model = new AuthorViewModel
            {
                Authors = Authors,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddAuthor(AuthorModel author)
        {
            var validationResult = _AuthorValidator.Validate(author);
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
                DateTime? dateOfBirth = null;
                if (!string.IsNullOrEmpty(author.DateOfBirth))
                {
                    if (DateTime.TryParse(author.DateOfBirth, out var parsedDate))
                    {
                        dateOfBirth = parsedDate;
                    }
                    else
                    {
                        ModelState.AddModelError("DateOfBirth", "Некорректная дата.");
                        return View(author); 
                    }
                }

                var newAuthor = new AuthorModel
                {
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                    Country = author.Country,
                    DateOfBirth = dateOfBirth?.ToString("yyyy-MM-dd") 
                };

                _AuthorService.AddAuthor(newAuthor);
                return RedirectToAction("Authors", "Author");
            }
        }

        [HttpGet("Author/AddAuthor")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult AddAuthor()
        {
            return View();
        }

        [HttpGet("Author/ViewAuthor/{AuthorId}")]
        public ActionResult ViewAuthor(int? AuthorId, int page = 1, int pageSize = 5)
        {
            int totalBooks;
            var author = _AuthorService.GetAuthorByIdForBooks(AuthorId, page, pageSize, out totalBooks);

            if (author == null)
            {
                return NotFound();
            }

            var model = new AuthorViewModel
            {
                Author = author,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalBooks / (double)pageSize)
            };

            return View(model);
        }



        [HttpPost]
        public ActionResult UpdateAuthor(AuthorModel author)
        {
            var validationResult = _AuthorValidator.Validate(author);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            DateTime? dateOfBirth = null;
            if (!string.IsNullOrEmpty(author.DateOfBirth))
            {
                if (DateTime.TryParse(author.DateOfBirth, out var parsedDate))
                {
                    dateOfBirth = parsedDate;
                }
                else
                {
                    ModelState.AddModelError("DateOfBirth", "Некорректная дата.");
                    return View(author); 
                }
            }

            _AuthorService.UpdateAuthor(author);
            return RedirectToAction("ViewAuthor", "Author", new { id = author.Id });
        }


        [HttpGet("Author/UpdateAuthor/{AuthorId}")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult UpdateAuthor(int AuthorId)
        {
            var Author = _AuthorService.GetAuthorById(AuthorId);
            return View(Author);
        }

        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult DeleteAuthor(int Id)
        {
            _AuthorService.DeleteAuthor(Id);
            return RedirectToAction("Authors", "Author");
        }
    }
}
