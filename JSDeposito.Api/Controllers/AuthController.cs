using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
using JSDeposito.Core.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSDeposito.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            Guid? tokenAnonimo = null;

            if (Request.Cookies.TryGetValue("pedido_anonimo", out var token))
            {
                tokenAnonimo = Guid.Parse(token);
            }

            var result = _auth.Login(
                dto.Email,
                dto.Senha,
                tokenAnonimo
            );

            // apaga cookie após associação
            if (tokenAnonimo.HasValue)
                Response.Cookies.Delete("pedido_anonimo");

            return Ok(result);
        }

        [Authorize]
        [HttpPost("refresh")]
        public IActionResult Refresh(RefreshRequest request)
            => Ok(_auth.Refresh(request.RefreshToken));

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout(LogoutRequest request)
        {
            _auth.Logout(request.RefreshToken);
            return NoContent();
        }
    }

    
    public record RefreshRequest(string RefreshToken);
    public record LogoutRequest(string RefreshToken);


}
