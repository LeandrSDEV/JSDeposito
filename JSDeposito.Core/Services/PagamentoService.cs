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
    private readonly PixService _pixService;

    public PagamentoService(
        IPagamentoRepository pagamentoRepository,
        IPedidoRepository pedidoRepository,
        PixService pixService)
    {
        _pagamentoRepository = pagamentoRepository;
        _pedidoRepository = pedidoRepository;
        _pixService = pixService;
    }

    public CriarPagamentoResponseDto CriarPagamento(
    int pedidoId,
    TipoPagamento tipo,
    int usuarioId)
    {
        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new Exception("Pedido não encontrado");

        if (!pedido.UsuarioId.HasValue)
            throw new Exception("Pedido precisa estar associado a um usuário");

        if (pedido.UsuarioId != usuarioId)
            throw new Exception("Pedido não pertence ao usuário autenticado");

        pedido.ValidarParaPagamento();

        var pagamentoPendente =
            _pagamentoRepository.ObterPagamentoPendentePorPedido(pedidoId);

        if (pagamentoPendente != null)
            throw new Exception("Já existe um pagamento em andamento");

        var pagamento = new Pagamento(pedidoId, pedido.Total, tipo);
        _pagamentoRepository.Criar(pagamento);

        var response = new CriarPagamentoResponseDto
        {
            PagamentoId = pagamento.Id,
            Status = pagamento.Status
        };

        if (tipo == TipoPagamento.Pix)
        {
            var pix = _pixService.GerarPix(
                pedido.Total,
                $"Pedido #{pedido.Id}"
            );

            pagamento.DefinirReferencia(pix.TxId);

            _pagamentoRepository.Atualizar(pagamento);

            response.Pix = pix;
        }

        return response;
    }


    public void ConfirmarPagamento(int pedidoId)
    {
        var pagamento = _pagamentoRepository.ObterPagamentoPendentePorPedido(pedidoId);

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
        var pagamento = _pagamentoRepository.ObterPagamentoPendentePorPedido(pedidoId)
         ?? throw new Exception("Pagamento não encontrado");

        if (pagamento.Status != StatusPagamento.Pendente)
            throw new Exception("Pagamento não pode ser cancelado");

        pagamento.Cancelar();
        _pagamentoRepository.Atualizar(pagamento);
    }

    public void ProcessarWebhookPix(
    string referencia,
    decimal valor,
    string status)
    {
        if (status.ToLower() != "paid")
            return;

        // 🔎 Agora buscamos pela referência (TxId)
        var pagamento = _pagamentoRepository
            .ObterPorReferencia(referencia);

        if (pagamento == null)
            return; // idempotência

        if (pagamento.Valor != valor)
            throw new Exception("Valor divergente");

        pagamento.Confirmar();

        var pedido = _pedidoRepository.ObterPorId(pagamento.PedidoId)
            ?? throw new Exception("Pedido não encontrado");

        pedido.MarcarComoPago();

        _pagamentoRepository.Atualizar(pagamento);
        _pedidoRepository.Atualizar(pedido);
    }
}
