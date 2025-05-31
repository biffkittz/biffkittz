using Microsoft.AspNetCore.Mvc;
using ListedLinks.Models;
using ListedLinks.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ListedLinks.Controllers
{
    public class BookController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ListedLinksContext _context;

        public BookController(ILogger<HomeController> logger, ListedLinksContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var books = _context.Books.OrderBy(_ => _.Title.ToLower()).ToList<Book>();
            return View(books);
        }

        //[HttpPost]
        //public void Index([FromBody] Book book)
        //{
        //    _context.Books.Add(book);
        //    _context.SaveChanges();

        //    RedirectToAction("Index");
        //}

        public IActionResult Management()
        {
            return View();
        }

        [HttpPost]
        public void Management([FromBody] Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();

            RedirectToAction("Index");
        }

        [HttpPost, ActionName("DeleteBook")]
        public async Task<IActionResult> DeleteBook(string titleAuthorKey)
        {
            if (string.IsNullOrEmpty(titleAuthorKey))
            {
                return NotFound();
            }

            var parts = titleAuthorKey.Split(new[] { "^^^" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                return NotFound();
            }

            var bookToDelete = await _context.Books.FindAsync(parts[0], parts[1]);

            if (bookToDelete != null)
            {
                _context.Books.Remove(bookToDelete);
                await _context.SaveChangesAsync();
            } else {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
