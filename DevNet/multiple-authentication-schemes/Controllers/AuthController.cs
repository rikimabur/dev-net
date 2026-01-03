using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using multiple_authentication_schemes.Services;

namespace multiple_authentication_schemes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _tokenService;

        public AuthController(IJwtTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login/public")]
        public IActionResult LoginPublic([FromBody] User user)
        {
            var token = _tokenService.CreatePublicToken(user.Username ?? "");
            return Ok(new { Token = token });
        }

        [HttpPost("login/internal")]
        public IActionResult LoginInternal([FromBody] User user)
        {
            var token = _tokenService.CreateInternalToken(user.Username ?? "");
            return Ok(new { Token = token });
        }

        [HttpPost("loginCookie")]
        public async Task<IActionResult> LoginCookie([FromBody] User user)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username ?? string.Empty));
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1)
                });
            return Ok();
        }
        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized()
        {
            return Unauthorized();
        }
        [HttpGet("forbidden")]
        public IActionResult GetForbidden()
        {
            return Forbid();
        }

        [HttpGet("public")]
        public IActionResult GePublicUser()
        {
            return Ok(new { Message = $"Hello {GetUsername()} public" });
        }
        [HttpGet("internal")]
        public IActionResult GetInternalUser()
        {
            return Ok(new { Message = $"Hello {GetUsername()} is internal" });
        }
        private string? GetUsername()
        {
            return HttpContext.User.Claims
                .Where(x => x.Type == ClaimTypes.Name)
                .Select(x => x.Value)
                .FirstOrDefault();
        }
    }
    public class User
    {
        public string? Username { get; set; }
    }
}
