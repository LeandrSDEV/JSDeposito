using JSDeposito.Api.UserExtensions;
using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
    public IActionResult Criar(CriarPedidoDto dto)
    {
        var response = _pedidoService.CriarPedidoAnonimo(dto);

        Response.Cookies.Append(
            "pedido_anonimo",
            response.TokenAnonimo.ToString(),
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            }
        );

        return Ok(response);
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

    [Authorize(Roles = "Cliente")]
    [HttpPost("{pedidoId}/frete/{enderecoId}")]
    public IActionResult AplicarFrete(int pedidoId, int enderecoId)
    {
        _pedidoService.AplicarFrete(pedidoId, enderecoId);
        return NoContent();
    }

    [HttpPost("{pedidoId}/cancelar")]
    public IActionResult Cancelar(int pedidoId)
    {
        _pedidoService.CancelarPedido(pedidoId);
        return NoContent();
    }

    [HttpPost("{pedidoId}/cupom")]
    public IActionResult AplicarCupom(
    int pedidoId,
    AplicarCupomDto dto)
    {
        _pedidoService.AplicarCupom(pedidoId, dto.CodigoCupom);
        return NoContent();
    }

}
