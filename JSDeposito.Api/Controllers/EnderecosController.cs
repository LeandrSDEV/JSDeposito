using JSDeposito.Api.UserExtensions;
using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSDeposito.Api.Controllers;

[ApiController]
[Route("api/enderecos")]
public class EnderecosController : ControllerBase
{
    private readonly EnderecoService _service;

    public EnderecosController(EnderecoService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] EnderecoDto dto)
    {
        var usuarioId = User.GetUserId();
        await _service.CriarEndereco(dto, usuarioId);
        return Ok();
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet]
    public IActionResult Listar()
    {
        var usuarioId = User.GetUserId();
        var enderecos = _service.ListarAtivosPorUsuario(usuarioId)
            .Select(e => new
            {
                id = e.Id,
                rua = e.Rua,
                numero = e.Numero,
                bairro = e.Bairro,
                cidade = e.Cidade,
                latitude = e.Latitude,
                longitude = e.Longitude
            })
            .ToList();

        return Ok(enderecos);
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet("{enderecoId:int}")]
    public IActionResult Obter(int enderecoId)
    {
        var usuarioId = User.GetUserId();
        var e = _service.ObterPorIdDoUsuario(enderecoId, usuarioId);

        return Ok(new
        {
            id = e.Id,
            rua = e.Rua,
            numero = e.Numero,
            bairro = e.Bairro,
            cidade = e.Cidade,
            latitude = e.Latitude,
            longitude = e.Longitude
        });
    }

}
