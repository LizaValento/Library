using Business.Models;
using Business.Services.Classes;
using Business.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tasks.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IValidator<LoginModel> _loginValidator; 
        private readonly IValidator<RegisterModel> _registerValidator;

        public AccountController(IUserService userService, ITokenService tokenService, IValidator<LoginModel> loginValidator, IValidator<RegisterModel> registerValidator)
        {
            _tokenService = tokenService;
            _userService = userService;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
        }

        [HttpGet("Account/Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel userModel)
        {
            var validationResult = _registerValidator.Validate(userModel);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ModelState); 
            }

            try
            {
                _userService.RegisterUser(userModel);
                return RedirectToAction("Login");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(userModel);
            }
        }

        [HttpGet("Account/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel userModel)
        {
            var validationResult = _loginValidator.Validate(userModel);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            try
            {
                var token = _userService.Authenticate(userModel);
                Response.Cookies.Append("AccessToken", token.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(1)
                });
                Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });


                return RedirectToAction("Main", "Book");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Refresh([FromBody] string refreshToken)
        {
            try
            {
                var tokenModel = _tokenService.RefreshToken(refreshToken);

                Response.Cookies.Append("AccessToken", tokenModel.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(1)
                });
                Response.Cookies.Append("RefreshToken", tokenModel.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });

                return Ok(tokenModel);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");

            return RedirectToAction("Main", "Book");
        }
    }
}
