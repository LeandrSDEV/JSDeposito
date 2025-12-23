using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface IPagamentoRepository
{
    void Criar(Pagamento pagamento);
    Pagamento? ObterPagamentoPendentePorPedido(int pedidoId);
    void Atualizar(Pagamento pagamento);
}
