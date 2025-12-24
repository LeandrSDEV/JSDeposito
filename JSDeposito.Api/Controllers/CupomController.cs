using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSDeposito.Api.Controllers;

[ApiController]
[Route("api/cupons")]
[Authorize(Roles = "Admin")]
public class CupomController : ControllerBase
{
    private readonly ICupomRepository _cupomRepository;

    public CupomController(ICupomRepository cupomRepository)
    {
        _cupomRepository = cupomRepository;
    }

    [HttpPost]
    public IActionResult Criar([FromBody] CriarCupomRequestDto dto)
    {
        var existente = _cupomRepository.ObterPorCodigo(dto.Codigo);

        if (existente != null)
            return BadRequest("Já existe um cupom com esse código.");

        var cupom = new Cupom(
            dto.Codigo,
            dto.ValorDesconto,
            dto.Percentual,
            dto.ValidoAte,
            dto.LimiteUso
        );

        _cupomRepository.Criar(cupom);

        return Created("", new
        {
            cupom.Codigo,
            cupom.ValidoAte,
            cupom.LimiteUso
        });
    }
}
