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

    public object CriarPagamento(int pedidoId, TipoPagamento tipo, int usuarioId)
    {
        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new Exception("Pedido não encontrado");

        if (!pedido.UsuarioId.HasValue)
        {
            pedido.AssociarUsuario(usuarioId);
            _pedidoRepository.Atualizar(pedido);
        }

        if (pedido.UsuarioId == null)
            throw new Exception("Pedido precisa estar associado a um usuário");

        if (pedido.UsuarioId != usuarioId)
            throw new Exception("Pedido não pertence ao usuário autenticado");

        pedido.ValidarParaPagamento();

        var pagamentoExistente = _pagamentoRepository.ObterPorPedido(pedidoId);
        if (pagamentoExistente != null)
            return pagamentoExistente;

        var pagamento = new Pagamento(pedidoId, pedido.Total, tipo);
        _pagamentoRepository.Criar(pagamento);

        if (tipo == TipoPagamento.Pix)
        {
            var pix = _pixService.GerarPix(
                pedido.Total,
                $"Pedido #{pedido.Id}"
            );

            return new
            {
                pagamento.Id,
                pagamento.Status,
                Pix = pix
            };
        }

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
