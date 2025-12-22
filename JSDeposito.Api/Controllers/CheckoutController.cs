using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    [HttpPost]
    public IActionResult Checkout([FromBody] CheckoutDto dto)
    {
        _checkoutService.RealizarCheckout(dto);
        return Ok(new { message = "Checkout realizado com sucesso" });
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost("{pedidoId}/finalizar")]
    public IActionResult FinalizarPedido(int pedidoId)
    {
        var clienteId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        _checkoutService.Finalizar(pedidoId, clienteId);

        return NoContent();
    }
}
