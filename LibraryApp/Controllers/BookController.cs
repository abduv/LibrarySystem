using Microsoft.AspNetCore.Mvc;
using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BookController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        public IActionResult Index() => IsAdmin() ? View(_context.Books.ToList()) : RedirectToAction("Index", "Home");

        // МЕТОД ДОБАВЛЕНИЯ (GET)
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        // МЕТОД ДОБАВЛЕНИЯ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (imageFile != null)
            {
                book.ImagePath = await SaveImage(imageFile);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult DeleteBook(int id)
        {
            if (!IsAdmin()) return BadRequest();
            var book = _context.Books.Find(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        // МЕТОД РЕДАКТИРОВАНИЯ (GET)
        public IActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // МЕТОД РЕДАКТИРОВАНИЯ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id != book.Id) return NotFound();

            try
            {
                if (imageFile != null)
                {
                    book.ImagePath = await SaveImage(imageFile);
                }
                else
                {
                    // Сохраняем старую картинку, если новую не выбрали
                    var oldBook = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                    book.ImagePath = oldBook?.ImagePath;
                }

                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(e => e.Id == book.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string path = Path.Combine(wwwRootPath, "images", "books");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return "/images/books/" + fileName;
        }

        public IActionResult ManageOrders()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View(_context.Orders.Include(o => o.Book).OrderByDescending(o => o.OrderDate).ToList());
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status;
                var book = _context.Books.Find(order.BookId);
                if (book != null) book.IsAvailable = (status != "Approved");
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(ManageOrders));
        }

        [HttpPost]
        public IActionResult DeleteOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                if (order.Status == "Approved")
                {
                    var book = _context.Books.Find(order.BookId);
                    if (book != null) book.IsAvailable = true;
                }
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(ManageOrders));
        }
    }
}