using EshopApplication.Orders;
using Microsoft.AspNetCore.Mvc;

namespace EshopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string email)
        {
            await orderService.Create(email);
            return Created();
        }
    }
}
