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
    public class RegisterValidator : AbstractValidator<RegisterModel>
    {
        public RegisterValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("Имя обязательно.")
                .Length(1, 50).WithMessage("Имя должно содержать от 1 до 50 символов.");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Фамилия обязательна.")
                .Length(1, 50).WithMessage("Фамилия должна содержать от 1 до 50 символов.");

            RuleFor(user => user.Nickname)
                .NotEmpty().WithMessage("Никнейм обязателен.")
                .Length(3, 20).WithMessage("Никнейм должен содержать от 3 до 20 символов.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Пароль обязателен.")
                .MinimumLength(6).WithMessage("Пароль должен содержать не менее 6 символов.");

        }
    }
}
