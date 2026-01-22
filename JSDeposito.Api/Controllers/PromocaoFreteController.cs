using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSDeposito.Api.Controllers;


[ApiController]
[Route("api/promocoes-frete")]
public class PromocaoFreteController : ControllerBase
{
    private readonly IPromocaoFreteRepository _repository;

    public PromocaoFreteController(IPromocaoFreteRepository repository)
    {
        _repository = repository;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Criar([FromBody] PromocaoFreteDto dto)
    {
        if (dto.Fim < dto.Inicio)
            return BadRequest("A data final não pode ser menor que a inicial");

        var promocao = new PromocaoFrete(
            dto.Nome,
            dto.Inicio,
            dto.Fim
        );

        _repository.Criar(promocao);

        return CreatedAtAction(
            nameof(Atualizar),
            new { id = promocao.Id },
            null
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public IActionResult ObterPorId(int id)
    {
        var promocao = _repository.ObterPorId(id);

        if (promocao == null)
            return NotFound();

        return Ok(promocao);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Listar()
    {
        var promocoes = _repository.Listar();
        return Ok(promocoes);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult Atualizar(int id, [FromBody] PromocaoFreteDto dto)
    {
        var promocao = _repository.ObterPorId(id);

        if (promocao == null)
            return NotFound("Promoção não encontrada");

        try
        {
            promocao.Atualizar(
                dto.Nome,
                dto.Inicio,
                dto.Fim,
                dto.Ativa
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        _repository.Atualizar(promocao);

        return NoContent();
    }
}