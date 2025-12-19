using JSDeposito.Core.Entities;
using JSDeposito.Core.Enums;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Core.Services;

public class PagamentoService
{
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IPedidoRepository _pedidoRepository;

    public PagamentoService(
        IPagamentoRepository pagamentoRepository,
        IPedidoRepository pedidoRepository)
    {
        _pagamentoRepository = pagamentoRepository;
        _pedidoRepository = pedidoRepository;
    }

    public Pagamento CriarPagamento(int pedidoId, TipoPagamento tipo)
    {
        var pedido = _pedidoRepository.ObterPorId(pedidoId);

        if (pedido.Status != PedidoStatus.Criado)
            throw new Exception("Pedido não pode ser pago");

        var pagamento = new Pagamento(
            pedidoId,
            pedido.Total,
            tipo
        );

        _pagamentoRepository.Criar(pagamento);
        return pagamento;
    }

    public void ConfirmarPagamento(int pedidoId)
    {
        var pagamento = _pagamentoRepository.ObterPorPedido(pedidoId);

        pagamento.Confirmar();

        var pedido = _pedidoRepository.ObterPorId(pedidoId);
        pedido.MarcarComoPago();
    }
}
