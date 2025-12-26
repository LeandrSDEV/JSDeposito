using JSDeposito.Core.Enums;
using JSDeposito.Core.Reports.DTOs;
using JSDeposito.Core.Reports.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JSDeposito.Api.Data
{
    public class RelatorioRepository : IRelatorioRepository
    {
        private readonly AppDbContext _context;

        public RelatorioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FaturamentoPeriodoDto>> ObterFaturamentoPorPeriodo(
        DateTime inicio,
        DateTime fim)
        {
            return await _context.Pedidos
                .Where(p =>
                    p.DataCriacao >= inicio &&
                    p.DataCriacao <= fim &&
                    p.Status == PedidoStatus.Pago)
                .GroupBy(p => p.DataCriacao.Date)
                .Select(g => new FaturamentoPeriodoDto
                {
                    Data = g.Key,
                    ValorTotal = g.Sum(p =>
                        p.Itens.Sum(i => i.PrecoUnitario * i.Quantidade)
                        + p.ValorFrete
                        - p.Desconto
                    )
                })
                .OrderBy(x => x.Data)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProdutoMaisVendidoDto>>
    ProdutosMaisVendidosAsync(DateTime inicio, DateTime fim, int top)
        {
            return await _context.Pedidos
                .Where(p =>
                    p.Status == PedidoStatus.Pago &&
                    p.DataCriacao >= inicio &&
                    p.DataCriacao <= fim)
                .SelectMany(p => p.Itens)
                .GroupBy(i => new { i.ProdutoId, i.NomeProduto })
                .Select(g => new ProdutoMaisVendidoDto(
                    g.Key.ProdutoId,
                    g.Key.NomeProduto,
                    g.Sum(x => x.Quantidade),
                    g.Sum(x => x.Quantidade * x.PrecoUnitario)
                ))
                .OrderByDescending(x => x.QuantidadeVendida)
                .Take(top)
                .ToListAsync();
        }

        public async Task<decimal> ObterTicketMedioAsync(DateTime inicio, DateTime fim)
        {
            return await _context.Pedidos
                .Where(p =>
                    p.Status == PedidoStatus.Pago &&
                    p.DataCriacao >= inicio &&
                    p.DataCriacao <= fim)
                .AverageAsync(p => p.Total);
        }
    }

}
