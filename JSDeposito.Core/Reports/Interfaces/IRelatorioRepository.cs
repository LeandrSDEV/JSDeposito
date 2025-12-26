using JSDeposito.Core.Reports.DTOs;

namespace JSDeposito.Core.Reports.Interfaces
{
    public interface IRelatorioRepository
    {
        Task<List<FaturamentoPeriodoDto>> ObterFaturamentoPorPeriodo(
        DateTime inicio,
        DateTime fim);
        Task<IEnumerable<ProdutoMaisVendidoDto>> ProdutosMaisVendidosAsync(
            DateTime inicio,
            DateTime fim,
            int top = 10
        );
        Task<decimal> ObterTicketMedioAsync(DateTime inicio, DateTime fim);
    }
}
