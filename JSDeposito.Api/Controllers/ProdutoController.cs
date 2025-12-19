using JSDeposito.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace JSDeposito.Api.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutoController : ControllerBase
{
    private readonly EstoqueService _estoqueService;

    public ProdutoController(EstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    [HttpPost("{id}/baixar")]
    public IActionResult Baixar(int id, int quantidade)
    {
        _estoqueService.Baixar(id, quantidade);
        return Ok();
    }
}
