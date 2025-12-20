using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace JSDeposito.Api.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidoController : ControllerBase
{
    private readonly PedidoService _pedidoService;

    public PedidoController(PedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpPost]
    public IActionResult Criar(CriarPedidoDto dto)
    {
        var pedido = _pedidoService.CriarPedido(dto);
        return Ok(pedido);
    }

    [HttpGet("{pedidoId}")]
    public IActionResult Obter(int pedidoId)
    {
        var pedido = _pedidoService.ObterPedido(pedidoId);

        if (pedido == null)
            return NotFound("Pedido não encontrado");

        return Ok(pedido);
    }

    [HttpPost("{pedidoId}/itens")]
    public IActionResult Adicionar(int pedidoId, AdicionarItemPedidoDto dto)
    {
        _pedidoService.AdicionarItem(pedidoId, dto);
        return NoContent();
    }

    [HttpDelete("{pedidoId}/itens/{itemId}")]
    public IActionResult Remover(int pedidoId, int itemId)
    {
        _pedidoService.RemoverItem(pedidoId, itemId);
        return NoContent();
    }

    [HttpPost("{pedidoId}/frete/{enderecoId}")]
    public IActionResult AplicarFrete(int pedidoId, int enderecoId)
    {
        _pedidoService.AplicarFrete(pedidoId, enderecoId);
        return NoContent();
    }
}
