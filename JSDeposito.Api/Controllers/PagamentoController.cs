using JSDeposito.Api.UserExtensions;
using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSDeposito.Api.Controllers;

[ApiController]
[Route("api/pagamentos")]
public class PagamentoController : ControllerBase
{
    private readonly PagamentoService _pagamentoService;

    public PagamentoController(PagamentoService pagamentoService)
    {
        _pagamentoService = pagamentoService;
    }

    [HttpPost("{pedidoId}/pagamento")]
    public IActionResult CriarPagamento(
    int pedidoId,
    CriarPagamentoDto dto)
    {
        var usuarioId = User.GetUserId();

        var resultado = _pagamentoService.CriarPagamento(
            pedidoId,
            dto.Tipo,
            usuarioId
        );

        return Ok(resultado);
    }

    [Authorize(Policy = "ConfirmarPagamento")]
    [HttpPost("{pedidoId}/confirmar")]
    public IActionResult Confirmar(int pedidoId)
    {
        _pagamentoService.ConfirmarPagamentoAsync(pedidoId);
        return Ok();
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost("{pedidoId}/cancelar")]
    public IActionResult Cancelar(int pedidoId)
    {
        _pagamentoService.CancelarPagamento(pedidoId);
        return NoContent();
    }

}
