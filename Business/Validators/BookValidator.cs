using Business.Models;
using Data.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Validators
{
    public class BookValidator : AbstractValidator<BookModel>
    {
        public BookValidator()
        {
            RuleFor(book => book.Name)
             .NotEmpty().WithMessage("Название обязательно.")
             .Length(1, 100).WithMessage("Название должно быть от 1 до 100 символов.");

            RuleFor(book => book.ISBN)
                .NotEmpty().WithMessage("ISBN обязателен.")
                .Length(10, 50).WithMessage("ISBN должен быть от 10 до 50 символов.");

            RuleFor(book => book.Genre)
                .NotEmpty().WithMessage("Жанр обязателен.");

            RuleFor(book => book.Description)
                .MaximumLength(500).WithMessage("Описание не должно превышать 500 символов.");

            RuleFor(book => book.BookImage)
                .Matches(@"^(file:\/\/|C:\\|D:\\|E:\\|F:\\).*\.(jpg|jpeg|png|gif)$")
                .WithMessage("Изображение книги должно быть действительным URL или локальным файлом, заканчивающимся на .jpg, .jpeg, .png или .gif.");

        }
    }
}
