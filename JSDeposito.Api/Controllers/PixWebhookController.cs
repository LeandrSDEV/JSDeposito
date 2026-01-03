using JSDeposito.Core.Configurations;
using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace JSDeposito.Api.Controllers;

[ApiController]
[Route("api/webhooks/pix")]
public class WebhookController : ControllerBase
{
    private readonly PagamentoService _pagamentoService;
    private readonly PixWebhookConfig _config;

    public WebhookController(
        PagamentoService pagamentoService,
        IOptions<PixWebhookConfig> config)
    {
        _pagamentoService = pagamentoService;
        _config = config.Value;
    }

    [EnableRateLimiting("pix")]
    [HttpPost]
    public IActionResult ReceberPix(
        [FromBody] PixWebhookDto dto,
        [FromHeader(Name = "X-Webhook-Token")] string token)
    {
        if (token != _config.Secret)
            return Unauthorized();

        _pagamentoService.ProcessarWebhookPix(
            dto.Referencia,
            dto.Valor,
            dto.Status
        );

        return Ok();
    }
}
