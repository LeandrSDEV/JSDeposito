using JSDeposito.Core.Enums;
using JSDeposito.Core.Services;
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

    [HttpPost("{pedidoId}")]
    public IActionResult Criar(int pedidoId, TipoPagamento tipo)
    {
        var pagamento = _pagamentoService.CriarPagamento(pedidoId, tipo);
        return Ok(pagamento);
    }

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
