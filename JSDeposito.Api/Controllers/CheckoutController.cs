using JSDeposito.Api.UserExtensions;
using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSDeposito.Api.Controllers;

[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly CheckoutService _checkoutService;

    public CheckoutController(CheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost("checkout")]
    public IActionResult Checkout([FromBody] CheckoutDto dto)
    {
        var usuarioId = User.GetUserId();
        _checkoutService.RealizarCheckout(dto, usuarioId);
        return Ok();
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost("{pedidoId:int}/finalizar")]
    public IActionResult FinalizarPedido(int pedidoId)
    {
        var clienteId = User.GetUserId();
        _checkoutService.Finalizar(pedidoId, clienteId);
        return NoContent();
    }
}
