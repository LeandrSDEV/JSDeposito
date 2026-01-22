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
}
