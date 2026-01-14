using Microsoft.AspNetCore.Mvc;
using Server_Sent_Events.Models;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Server_Sent_Events.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MetricsGenerator _generator;
        public HomeController(ILogger<HomeController> logger, MetricsGenerator generator)
        {
            _logger = logger;
            _generator = generator;
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
        public async Task Stream()
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-control", "no-cache");
            // an HTTP response header, primarily used with NGINX, that controls response buffering for proxied content
            Response.Headers.Append("X-Accel-Buffering", "no");

            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                var metric = _generator.Generate();
                var json = System.Text.Json.JsonSerializer.Serialize(metric);

                await Response.WriteAsync($"data: {json}\n\n");
                await Response.Body.FlushAsync();

                await Task.Delay(1000);
            }
        }
    }
    public record Metric(string Name, double Value, DateTime Timestamp);
    public class MetricsGenerator
    {
        private readonly Random _random = new Random();
        public Metric Generate()
        {
            return new Metric(
                Name: "CPU_Usage",
                Value: Math.Round(_random.NextDouble() * 100, 2),
                Timestamp: DateTime.UtcNow
            );
        }
    }
}
