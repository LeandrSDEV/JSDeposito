using JSDeposito.Core.DTOs;
using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _service;

    public ClientesController(ClienteService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost]
    public IActionResult Criar([FromBody] ClienteDto dto)
    {
        var cliente = _service.CriarCliente(dto.Nome, dto.Email, dto.Telefone);
        return Ok(cliente);
    }
}
