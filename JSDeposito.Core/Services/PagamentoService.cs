using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Enums;
using JSDeposito.Core.Interfaces;
using System.Security;

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
        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new Exception("Pedido não encontrado");


        pedido.ValidarParaPagamento();

        var pagamentoExistente = _pagamentoRepository.ObterPorPedido(pedidoId);

        if (pagamentoExistente != null)
            return pagamentoExistente; // 🔥 IDEMPOTÊNCIA

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

        if (pagamento == null)
            throw new Exception("Pagamento não encontrado");

        if (pagamento.Status != StatusPagamento.Pendente)
            throw new Exception("Pagamento não pode ser confirmado");

        pagamento.Confirmar();

        var pedido = _pedidoRepository.ObterPorId(pedidoId);

        if (pedido == null)
            throw new Exception("Pedido não encontrado");

        pedido.MarcarComoPago();

        _pagamentoRepository.Atualizar(pagamento);
        _pedidoRepository.Atualizar(pedido);
    }

    public void CancelarPagamento(int pedidoId)
    {
        var pagamento = _pagamentoRepository.ObterPorPedido(pedidoId);

        if (pagamento == null)
            throw new Exception("Pagamento não encontrado");

        pagamento.Cancelar();

        _pagamentoRepository.Atualizar(pagamento);

        var pedido = _pedidoRepository.ObterPorId(pedidoId);

        if (pedido != null && pedido.Status == PedidoStatus.Criado)
            pedido.Cancelar();
    }

}
