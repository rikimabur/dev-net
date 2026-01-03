using Microsoft.AspNetCore.Mvc;
using OrderApi.Commands;
using OrderApi.Mediator;
using OrderApi.Queries;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            [FromBody] CreateOrderCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.SendAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder([FromRoute]Guid orderId, CancellationToken cancellationToken)
        {
            var request = new GetOrderQuery(orderId);
            var result = await _mediator.SendAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}
