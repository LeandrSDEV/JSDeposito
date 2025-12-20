using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
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

    [HttpPost]
    public IActionResult Checkout([FromBody] CheckoutDto dto)
    {
        _checkoutService.RealizarCheckout(dto);
        return Ok(new { message = "Checkout realizado com sucesso" });
    }
}
