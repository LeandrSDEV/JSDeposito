using JSDeposito.Core.Reports.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JSDeposito.Api.Controllers;


[ApiController]
[Route("api/relatorios")]
public class RelatoriosController : ControllerBase
{
    private readonly RelatorioService _service;
    private readonly ExportacaoRelatorioService _exportacao;

    public RelatoriosController(RelatorioService service, ExportacaoRelatorioService exportacao)
    {
        _service = service;
        _exportacao = exportacao;
    }

    [HttpGet("faturamento/export")]
public async Task<IActionResult> ExportarFaturamento(
    [FromQuery] DateTime inicio,
    [FromQuery] DateTime fim,
    [FromQuery] string formato = "csv")
{
    var dados = await _service.FaturamentoPorPeriodo(inicio, fim);

    if (!dados.Any())
        return NoContent();

    if (formato.ToLower() == "excel")
    {
        var arquivo = _exportacao.ExportarExcel(dados);
        return File(
            arquivo,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"faturamento_{inicio:yyyyMMdd}_{fim:yyyyMMdd}.xlsx"
        );
    }

    // CSV padrão
    var csv = _exportacao.ExportarCsv(dados);
    return File(
        csv,
        "text/csv",
        $"faturamento_{inicio:yyyyMMdd}_{fim:yyyyMMdd}.csv"
    );
}

    [HttpGet("produtos-mais-vendidos")]
    public async Task<IActionResult> ProdutosMaisVendidos(
        DateTime inicio,
        DateTime fim,
        int top = 10)
        => Ok(await _service.ProdutosMaisVendidos(inicio, fim, top));

    [HttpGet("ticket-medio")]
    public async Task<IActionResult> TicketMedio(
        DateTime inicio,
        DateTime fim)
        => Ok(await _service.TicketMedio(inicio, fim));
}