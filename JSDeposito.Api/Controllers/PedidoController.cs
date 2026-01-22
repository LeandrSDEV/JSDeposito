using JSDeposito.Api.UserExtensions;
using JSDeposito.Core.DTOs;
using JSDeposito.Core.Exceptions;
using JSDeposito.Core.Interfaces;
using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSDeposito.Api.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidoController : ControllerBase
{
    private readonly PedidoService _pedidoService;
    private readonly IPedidoRepository _pedidoRepository;

    public PedidoController(PedidoService pedidoService, IPedidoRepository pedidoRepository)
    {
        _pedidoService = pedidoService;
        _pedidoRepository = pedidoRepository;
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Criar([FromBody] CriarPedidoDto? dto)
    {
        dto ??= new CriarPedidoDto();

        // 🔒 SE USUÁRIO ESTIVER LOGADO, NÃO CRIA OUTRO PEDIDO
        if (User.Identity?.IsAuthenticated == true)
        {
            var usuarioId = User.GetUserId();
            var pedidoExistente = _pedidoRepository.ObterPedidoAbertoDoUsuario(usuarioId);

            if (pedidoExistente != null)
                return Ok(new { pedidoId = pedidoExistente.Id });
        }

        // 🔁 evita múltiplos pedidos anônimos
        if (Request.Cookies.TryGetValue("pedido_anonimo", out var tokenStr) &&
            Guid.TryParse(tokenStr, out var tokenExistente))
        {
            var pedidoExistente = _pedidoRepository.ObterPorTokenAnonimo(tokenExistente);
            if (pedidoExistente != null)
                return Ok(new { pedidoId = pedidoExistente.Id });
        }

        var response = _pedidoService.CriarPedidoAnonimo(dto);

        Response.Cookies.Append(
            "pedido_anonimo",
            response.TokenAnonimo.ToString(),
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

        return Ok(new { pedidoId = response.PedidoId });
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
        _pedidoService.ValidarAcessoAoPedido(
    pedidoId,
    User.Identity?.IsAuthenticated == true ? User.GetUserId() : null,
    ObterTokenAnonimo()
);
        _pedidoService.AdicionarItem(pedidoId, dto);
        return Ok();
    }


    [HttpDelete("{pedidoId}/produtos/{produtoId}")]
    public IActionResult RemoverProduto(int pedidoId, int produtoId)
    {
        _pedidoService.ValidarAcessoAoPedido(
    pedidoId,
    User.Identity?.IsAuthenticated == true ? User.GetUserId() : null,
    ObterTokenAnonimo()
);
        _pedidoService.RemoverItemPorProduto(pedidoId, produtoId);
        return NoContent();
    }

    [HttpPut("{pedidoId}/itens/{produtoId}")]
    public IActionResult AlterarQuantidade(
        int pedidoId,
        int produtoId,
        AlterarQuantidadeDto dto)
    {
        _pedidoService.ValidarAcessoAoPedido(
    pedidoId,
    User.Identity?.IsAuthenticated == true ? User.GetUserId() : null,
    ObterTokenAnonimo()
);
        _pedidoService.AlterarQuantidade(pedidoId, produtoId, dto.Quantidade);
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

    [HttpDelete("{pedidoId}")]
    public IActionResult Excluir(int pedidoId)
    {
        var pedido = _pedidoService.ObterPedido(pedidoId)
            ?? throw new NotFoundException("Pedido não encontrado");

        _pedidoService.ExcluirPedido(pedidoId);

        return NoContent();
    }

    [Authorize]
    [HttpGet("pedido-atual")]
    public IActionResult PedidoAtual()
    {
        var usuarioId = User.GetUserId();
        var pedido = _pedidoService.ObterPedidoAbertoDoUsuario(usuarioId);

        if (pedido == null)
            return NoContent();

        return Ok(pedido);
    }

    [HttpPost("associar-carrinho")]
    public IActionResult AssociarCarrinho()
    {
        var token = Request.Cookies["pedido_anonimo"];
        if (string.IsNullOrEmpty(token)) return Ok();

        var usuarioId = User.GetUserId();

        var conflito = _pedidoService.VerificarOuAssociarCarrinho(
            Guid.Parse(token),
            usuarioId);

        if (conflito != null)
            return Conflict(new
            {
                message = "Usuário já possui um carrinho ativo"
            });

        RemoverCookieAnonimo();
        return Ok();
    }

    [Authorize]
    [HttpPost("descartar-anonimo")]
    public IActionResult DescartarAnonimo()
    {
        var token = Request.Cookies["pedido_anonimo"];
        if (string.IsNullOrEmpty(token)) return Ok();

        var pedido = _pedidoRepository.ObterPorTokenAnonimo(Guid.Parse(token));
        if (pedido != null)
            _pedidoRepository.Remover(pedido);

        RemoverCookieAnonimo();
        return Ok();
    }

    [Authorize]
    [HttpPost("substituir")]
    public IActionResult SubstituirCarrinho()
    {
        var token = Request.Cookies["pedido_anonimo"];
        if (string.IsNullOrEmpty(token))
            return BadRequest();

        var usuarioId = User.GetUserId();

        var pedidoAnonimo = _pedidoRepository.ObterPorTokenAnonimo(Guid.Parse(token))
            ?? throw new NotFoundException("Pedido anônimo não encontrado");

        var pedidoUsuario = _pedidoRepository.ObterPedidoAbertoDoUsuario(usuarioId);
        if (pedidoUsuario != null)
            _pedidoRepository.Remover(pedidoUsuario);

        pedidoAnonimo.AssociarUsuario(usuarioId);
        pedidoAnonimo.RemoverTokenAnonimo();
        _pedidoRepository.Atualizar(pedidoAnonimo);

        RemoverCookieAnonimo();
        return Ok();
    }

    private void RemoverCookieAnonimo()
    {
        Response.Cookies.Append(
            "pedido_anonimo",
            "",
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                Secure = true,
                SameSite = SameSiteMode.None,
                HttpOnly = true
            });
    }

    private Guid? ObterTokenAnonimo()
    {
        if (Request.Cookies.TryGetValue("pedido_anonimo", out var token) &&
            Guid.TryParse(token, out var guid))
        {
            return guid;
        }

        return null;
    }

}
