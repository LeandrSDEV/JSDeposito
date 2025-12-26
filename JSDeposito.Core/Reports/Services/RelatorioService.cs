using JSDeposito.Core.Reports.Interfaces;
using JSDeposito.Core.Reports.DTOs;

namespace JSDeposito.Core.Reports.Services
{
    public class RelatorioService
    {
        private readonly IRelatorioRepository _repository;

        public RelatorioService(IRelatorioRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FaturamentoPeriodoDto>> FaturamentoPorPeriodo(
        DateTime inicio,
        DateTime fim)
        {
            if (inicio > fim)
                throw new ArgumentException("Data inicial não pode ser maior que a final");

            return await _repository.ObterFaturamentoPorPeriodo(inicio, fim);
        }

        public Task<IEnumerable<ProdutoMaisVendidoDto>> ProdutosMaisVendidos(
            DateTime inicio,
            DateTime fim,
            int top = 10)
            => _repository.ProdutosMaisVendidosAsync(inicio, fim, top);

        public async Task<TicketMedioDto> TicketMedio(DateTime inicio, DateTime fim)
        {
            var valor = await _repository.ObterTicketMedioAsync(inicio, fim);
            return new TicketMedioDto(inicio, fim, valor);
        }


    }

}
