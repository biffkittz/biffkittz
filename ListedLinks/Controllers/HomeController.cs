using System.Collections;
using System.Diagnostics;
using ListedLinks.Hubs;
using ListedLinks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using static System.Net.Mime.MediaTypeNames;

namespace ListedLinks.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ListedLinksContext _context;
        private readonly IHubContext<SaaSActivityHub> _hubContext;

        public HomeController(ILogger<HomeController> logger, ListedLinksContext context, IHubContext<SaaSActivityHub> hubContext)
        {
            _logger = logger;
            _context = context;
            _hubContext = hubContext;
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

        public IActionResult Books()
        {
            return View();
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
            var comments = _context.Comments.Take(500).ToList<Comment>();
            return View(comments);
        }

        public IActionResult ScreenConnectActivity()
        {
            var comments = _context.Comments.Take(500).ToList<Comment>();
            return View(comments);
        }

        public IActionResult Monitor()
        {
            var comments = _context.Comments.Take(5000).ToList<Comment>();
            return View(comments);
        }

        [HttpPost]
        public IResult Monitor([FromBody] string commentText)
        {
            if (commentText != null /*&& commentText.StartsWith("ScreenConnect-d7f4d503647e227df8583449a07a4000 ")*/)
            {
                //var _commentText = commentText.Replace("-d7f4d503647e227df8583449a07a4000", "");
                var comment = new Comment
                {
                    Text = commentText,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Comments.Add(comment);
                _context.SaveChanges();

                _hubContext.Clients.All.SendAsync(
                    "ReceiveMessage",
                    "ScreenConnect",
                    commentText
                );
            }

            return Results.Ok();
        }

        public IActionResult Ramblings()
        {
            return View();
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
