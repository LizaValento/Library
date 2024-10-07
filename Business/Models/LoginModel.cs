using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Data.Entities;

namespace Business.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Введите никнейм")]
        public string Nickname { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
    }
}
