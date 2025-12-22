using JSDeposito.Core.Enums;
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

    [Authorize(Roles = "Cliente")]
    [HttpPost("{pedidoId}")]
    public IActionResult Criar(int pedidoId, TipoPagamento tipo)
    {
        var pagamento = _pagamentoService.CriarPagamento(pedidoId, tipo);
        return Ok(pagamento);
    }

    [Authorize]
    [Authorize(Roles = "Admin")]
    [HttpPost("{pedidoId}/confirmar")]
    public IActionResult Confirmar(int pedidoId)
    {
        _pagamentoService.ConfirmarPagamento(pedidoId);
        return Ok();
    }

    [HttpPost("{pedidoId}/cancelar")]
    public IActionResult Cancelar(int pedidoId)
    {
        _pagamentoService.CancelarPagamento(pedidoId);
        return NoContent();
    }

}
