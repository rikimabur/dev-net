using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace multiple_authentication_schemes.Controllers
{
    [Authorize(AuthenticationSchemes = AuthSchemes.JwtPublic)]
    [ApiController]
    [Route("api/public/orders")]
    public class PublicOrdersController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Message = $"public" });
        }
    }
}
