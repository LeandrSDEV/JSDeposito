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
        return Ok();
    }

    [HttpDelete("{pedidoId}/produtos/{produtoId}")]
    public IActionResult RemoverPorProduto(int pedidoId, int produtoId)
    {
        _pedidoService.RemoverItemPorProduto(pedidoId, produtoId);
        return NoContent();
    }

    [HttpPost("{pedidoId}/frete/{enderecoId}")]
    public IActionResult AplicarFrete(int pedidoId, int enderecoId)
    {
        _pedidoService.AplicarFrete(pedidoId, enderecoId);
        return NoContent();
    }
}
