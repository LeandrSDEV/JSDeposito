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
        private readonly ILogger<AuthController> _logger;
        private readonly AuthService _authService;

        public AuthController(AuthService auth, ILogger<AuthController> logger, AuthService authService)
        {
            _auth = auth;
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            _auth.Register(request);
            return Created("", null);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            Guid? tokenAnonimoPedido = null;

            if (Request.Cookies.TryGetValue("pedido_anonimo", out var cookieValue)
                && Guid.TryParse(cookieValue, out var guid))
            {
                tokenAnonimoPedido = guid;
            }

            var response = _authService.Login(
                dto.Email,
                dto.Senha,
                tokenAnonimoPedido
            );

            // 🔥 remove cookie APÓS associar
            if (tokenAnonimoPedido.HasValue)
            {
                Response.Cookies.Delete("pedido_anonimo");
            }

            return Ok(response);
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
