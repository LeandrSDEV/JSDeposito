using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace JSDeposito.Api.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutoController : ControllerBase
{
    private readonly ProdutoService _produtoService;

    public ProdutoController(ProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpPost]
    public IActionResult Criar(string nome, decimal preco, int estoqueInicial)
    {
        return Ok(_produtoService.Criar(nome, preco, estoqueInicial));
    }


    [HttpGet]
    public IActionResult Listar()
    {
        return Ok(_produtoService.Listar());
    }


    [HttpGet("{id}")]
    public IActionResult Obter(int id)
    {
        return Ok(_produtoService.Obter(id));
    }


    [HttpPut("{id}")]
    public IActionResult Atualizar(int id, AtualizarProdutoDto dto)
    {
        _produtoService.AtualizarProduto(id, dto);
        return NoContent();
    }


    [HttpPost("{id}/entrada-estoque")]
    public IActionResult EntradaEstoque(int id, int quantidade)
    {
        _produtoService.EntradaEstoque(id, quantidade);
        return NoContent();
    }


    [HttpPost("{id}/saida-estoque")]
    public IActionResult SaidaEstoque(int id, int quantidade)
    {
        _produtoService.SaidaEstoque(id, quantidade);
        return NoContent();
    }
}
