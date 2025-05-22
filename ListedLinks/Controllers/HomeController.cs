using System.Collections;
using System.Diagnostics;
using ListedLinks.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace ListedLinks.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ListedLinksContext _context;

        public HomeController(ILogger<HomeController> logger, ListedLinksContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (ip != null)
            {
                using (var dbContext = new ListedLinksContext())
                {
                    if (!dbContext.IPAddressStrings.Where(_ => String.Equals(_.IPAddress, ip)).Any())
                    {
                        dbContext.IPAddressStrings.Add(new IPAddressString { IPAddress = ip });
                        dbContext.SaveChanges();
                    }
                }
            }

            var linkGroups = _context.ListedLinks.OrderBy(_ => _.Title).ToList<ListedLink>();
            return View(linkGroups);
        }

        [HttpPost]
        public IActionResult Index([FromBody] Comment comment)
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
            var linkGroups = _context.ListedLinks.OrderBy(_ => _.Title).ToList<ListedLink>();

            return View(linkGroups);
        }

        public IActionResult Snoop()
        {
            var ipAddressStrings = _context.IPAddressStrings.Take(500).ToList<IPAddressString>();
            return View(ipAddressStrings);
        }

        public IActionResult LinkManagement()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LinkManagement([FromBody] ListedLink link)
        {
            _context.ListedLinks.Add(link);
            _context.SaveChanges();

            return View();
        }

        public IActionResult Comments()
        {
            var comments = _context.Comments.Take(1000).ToList<Comment>();
            return View(comments);
        }

        public IActionResult ScreenConnectActivity()
        {
            var comments = _context.Comments.Take(1000).ToList<Comment>();
            return View(comments);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Photos()
        {
            return View();
        }

        //public IActionResult React()
        //{
        //    return View(_comments);
        //}

        //[Route("comments")]
        //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        //public ActionResult Comments()
        //{
        //    return Json(_comments);
        //}

        //[Route("comments/new")]
        //[HttpPost]
        //public ActionResult AddComment(CommentModel comment)
        //{
        //    // Create a fake ID for this comment
        //    comment.Id = _comments.Count + 1;
        //    _comments.Add(comment);
        //    return Content("Success :)");
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
