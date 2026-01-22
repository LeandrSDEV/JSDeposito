using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
using JSDeposito.Core.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSDeposito.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public ActionResult<AuthResponse> Login([FromBody] LoginRequest req, [FromQuery] Guid? tokenAnonimoPedido)
    {
        var res = _authService.Login(req.Email, req.Senha, tokenAnonimoPedido);
        return Ok(res);
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest req)
    {
        _authService.Register(new JSDeposito.Core.DTOs.RegisterRequest(req.Nome, req.Email, req.Telefone, req.Senha));
        return StatusCode(201);
    }

    [Authorize]
    [HttpPost("refresh")]
    public ActionResult<AuthResponse> Refresh([FromBody] RefreshRequest req)
    {
        var res = _authService.Refresh(req.RefreshToken);
        return Ok(res);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout([FromBody] RefreshRequest req)
    {
        _authService.Logout(req.RefreshToken);
        return NoContent();
    }
}

public record LoginRequest(string Email, string Senha);

public record RegisterRequest(string Nome, string Email, string Telefone, string Senha);

public record RefreshRequest(string RefreshToken);