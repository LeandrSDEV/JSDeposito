using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Mvc;

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
    public IActionResult Criar([FromBody] EnderecoDto dto)
    {
        var endereco = _service.CriarEndereco(
            dto.ClienteId,
            dto.Rua,
            dto.Numero,
            dto.Bairro,
            dto.Cidade,
            dto.Latitude,
            dto.Longitude
        );

        return Ok(endereco);
    }
}
