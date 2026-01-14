using Microsoft.AspNetCore.Mvc;
using Server_Sent_Events.Models;
using System.Diagnostics;

namespace Server_Sent_Events.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult Stream(CancellationToken cancellationToken)
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-control", "no-cache");
            // an HTTP response header, primarily used with NGINX, that controls response buffering for proxied content
            Response.Headers.Append("X-Accel-Buffering", "tno");
            return View();
        }
    }   
    public record Metric(string Name, double Value, DateTime Timestamp);
    public class MetricsGenerator
    { 
        private readonly Random _random = new Random();
    }
}
