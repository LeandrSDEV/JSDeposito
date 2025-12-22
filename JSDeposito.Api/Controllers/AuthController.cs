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
        public IActionResult Login(LoginRequest request)
            => Ok(_auth.Login(request.Email, request.Senha));

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
