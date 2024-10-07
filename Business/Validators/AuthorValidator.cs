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
    public class AuthorValidator : AbstractValidator<AuthorModel>
    {
        public AuthorValidator()
        {
            RuleFor(author => author.FirstName)
             .NotEmpty().WithMessage("Имя обязательно.")
             .Length(1, 50).WithMessage("Имя должно быть от 1 до 50 символов.");

            RuleFor(author => author.LastName)
                .NotEmpty().WithMessage("Фамилия обязательна.")
                .Length(1, 50).WithMessage("Фамилия должна быть от 1 до 50 символов.");


            RuleFor(author => author.Country)
                .Length(1, 100).WithMessage("Страна должна быть от 1 до 100 символов.");
        }
    }
}
