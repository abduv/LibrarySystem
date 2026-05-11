using Microsoft.AspNetCore.Mvc;
using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context) => _context = context;

        public IActionResult Index(string searchString, string category)
        {
            var books = _context.Books.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
                books = books.Where(b => b.Title.Contains(searchString) || b.Author.Contains(searchString));
            if (!string.IsNullOrEmpty(category))
                books = books.Where(b => b.Category == category);

            return View(books.ToList());
        }

        public IActionResult Details(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();
            
            var userLogin = HttpContext.Session.GetString("UserName");
            ViewBag.ExistingOrder = _context.Orders.FirstOrDefault(o => o.BookId == id && o.UserLogin == userLogin && o.Status == "Pending");
            
            return View(book);
        }

        [HttpPost]
        public IActionResult OrderBook(int bookId)
        {
            var userLogin = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userLogin)) return RedirectToAction("Login", "Account");

            var order = new Order { BookId = bookId, UserLogin = userLogin, Status = "Pending", OrderDate = DateTime.Now };
            _context.Orders.Add(order);
            _context.SaveChanges();

            TempData["Message"] = HttpContext.Session.GetString("Language") == "kz" 
                ? "Өтінім жіберілді! Админнің жауабын күтіңіз." 
                : "Заявка отправлена! Ждите ответа админа.";
            return RedirectToAction("Details", new { id = bookId });
        }

        [HttpPost]
        public IActionResult CancelOrder(int bookId)
        {
            var userLogin = HttpContext.Session.GetString("UserName");
            var order = _context.Orders.FirstOrDefault(o => o.BookId == bookId && o.UserLogin == userLogin && o.Status == "Pending");
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
            return RedirectToAction("Details", new { id = bookId });
        }

        public IActionResult ChangeLanguage(string culture)
        {
            HttpContext.Session.SetString("Language", culture);
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }
    }
}