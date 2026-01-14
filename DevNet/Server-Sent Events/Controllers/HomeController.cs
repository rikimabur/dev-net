using Microsoft.AspNetCore.Mvc;
using Server_Sent_Events.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using System.Runtime.ConstrainedExecution;

namespace Server_Sent_Events.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MetricsGenerator _generator;
        public readonly SSEConnectionManager _manager;
        public HomeController(ILogger<HomeController> logger, MetricsGenerator generator, SSEConnectionManager manager)
        {
            _logger = logger;
            _generator = generator;
            _manager = manager;
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
        public async Task Stream(string? userId)
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-control", "no-cache");
            // an HTTP response header, primarily used with NGINX, that controls response buffering for proxied content
            Response.Headers.Append("X-Accel-Buffering", "no");
            var connectionId = string.IsNullOrEmpty(userId) ? Guid.NewGuid().ToString() : userId;
            _manager.Connections[connectionId] = Response;

            try
            {
                // Keep connection open until client disconnects
                await Task.Delay(Timeout.Infinite, HttpContext.RequestAborted);
            }
            catch (TaskCanceledException)
            {
                // Client disconnected
            }
            finally
            {
                _manager.Connections.TryRemove(userId, out _);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendToUser(string? userId)
        {
            var metric = _generator.Generate(userId);
            var json = JsonSerializer.Serialize(metric);

            foreach (var item in _manager.Connections.ToArray())
            {
                try
                {
                    await item.Value.WriteAsync($"data: {json}\n\n");
                    await item.Value.Body.FlushAsync();
                }
                catch
                {
                    _manager.Connections.TryRemove(item.Key, out _);
                }
            }

            return Ok();
        }

        [HttpGet]
        public async Task StreamAll()
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-control", "no-cache");
            // an HTTP response header, primarily used with NGINX, that controls response buffering for proxied content
            Response.Headers.Append("X-Accel-Buffering", "no");

            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                var metric = _generator.Generate(string.Empty);
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
        public Metric Generate(string? userId)
        {
            return new Metric(
                Name: string.IsNullOrEmpty(userId) ? "CPU_Usage" : $"CPU (User {userId})",
                Value: Math.Round(_random.NextDouble() * 100, 2),
                Timestamp: DateTime.UtcNow
            );
        }
    }
}
