using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface IPagamentoRepository
{
    void Criar(Pagamento pagamento);
    Pagamento ObterPorPedido(int pedidoId);
}
