using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[Authorize(Roles = "Cliente")]
[ApiController]
[Route("api/enderecos")]
public class EnderecosController : ControllerBase
{
    private readonly EnderecoService _service;

    public EnderecosController(EnderecoService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(EnderecoDto dto)
    {
        await _service.CriarEndereco(dto);
        return Ok();
    }
}
