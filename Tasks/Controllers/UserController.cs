using Business.Models;
using Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using System.Security.Claims;

namespace Tasks.Controllers
{
    public class UserController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly JWTSettings _jwtSettings;
        private readonly IUserService _userService;

        public UserController(IUserService userService, ITokenService tokenService, IOptions<JWTSettings> jwtSettings)
        {
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
            _userService = userService;
        }

        public ViewResult Index()
        {
            var users = _userService.GetUsers();
            return View(users);
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(UserModel user)
        {
            _userService.AddUser(user);
            return RedirectToAction("Index");
        }

        [HttpGet("User/AddUser")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpGet("User/ViewUserBooks")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult ViewUserBooks(int page = 1, int pageSize = 2)
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var (books, totalCount) = _userService.GetUserBooks(userId, page, pageSize);

            var model = new BooksViewModel
            {
                Books = books,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult UpdateUser(UserModel user)
        {
            _userService.UpdateUser(user);
            return RedirectToAction("Index");
        }

        [HttpGet("User/UpdateUser/{userId}")]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult UpdateUser(int userId)
        {
            var user = _userService.GetUserById(userId);
            return View(user);
        }

        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeAttribute))]
        public ActionResult DeleteUser(int userId)
        {
            _userService.DeleteUser(userId);
            return Json(new { result = true });
        }
    }
}
