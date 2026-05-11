using Microsoft.AspNetCore.Mvc;
using LibraryApp.Data;
using LibraryApp.Models;
using System.Linq;

namespace LibraryApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string login, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user != null)
            {
                // Сохраняем данные в сессию
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.Login);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (_context.Users.Any(u => u.Login == user.Login))
            {
                ViewBag.Error = "Этот логин уже занят";
                return View();
            }

            user.Role = "Client"; // По умолчанию все — клиенты
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}