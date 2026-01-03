using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace multiple_authentication_schemes.Controllers
{
    [Authorize(AuthenticationSchemes = AuthSchemes.JwtInternal)]
    [ApiController]
    [Route("api/internal/health")]
    public class InternalHealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Message = $"internal" });
        }
    }
}
