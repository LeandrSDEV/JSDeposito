using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Enums;
using JSDeposito.Core.Exceptions;
using JSDeposito.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security;

namespace JSDeposito.Core.Services;

public class PagamentoService
{
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly PixService _pixService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PagamentoService> _logger;

    public PagamentoService(
        IPagamentoRepository pagamentoRepository,
        IPedidoRepository pedidoRepository,
        PixService pixService,
        ILogger<PagamentoService> logger,
        IUnitOfWork unitOfWork)
    {
        _pagamentoRepository = pagamentoRepository;
        _pedidoRepository = pedidoRepository;
        _pixService = pixService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public CriarPagamentoResponseDto CriarPagamento(
    int pedidoId,
    TipoPagamento tipo,
    int usuarioId)
    {
        _logger.LogInformation(
            "Iniciando criação de pagamento | PedidoId: {PedidoId} | UsuarioId: {UsuarioId} | Tipo: {Tipo}",
            pedidoId, usuarioId, tipo);

        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new NotFoundException("Pedido não encontrado");

        if (!pedido.UsuarioId.HasValue)
            throw new BusinessException("Pedido precisa estar associado a um usuário");

        if (pedido.UsuarioId != usuarioId)
            throw new SecurityException("Pedido não pertence ao usuário autenticado");

        pedido.ValidarParaPagamento();

        var pagamentoPendente =
            _pagamentoRepository.ObterPagamentoPendentePorPedido(pedidoId);

        if (pagamentoPendente != null)
            throw new BusinessException("Já existe um pagamento em andamento");

        var pagamento = new Pagamento(pedidoId, pedido.Total, tipo);
        _pagamentoRepository.Criar(pagamento);

        _logger.LogInformation(
            "Pagamento criado | PagamentoId: {PagamentoId} | PedidoId: {PedidoId}",
            pagamento.Id, pedidoId);

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

            _logger.LogInformation(
                "PIX gerado | PagamentoId: {PagamentoId} | TxId: {TxId}",
                pagamento.Id, pix.TxId);

            response.Pix = pix;
        }

        return response;
    }


    public async Task ConfirmarPagamentoAsync(int pedidoId)
    {
        await _unitOfWork.BeginAsync();

        try
        {
            var pagamento = _pagamentoRepository
                .ObterPagamentoPendentePorPedido(pedidoId)
                ?? throw new NotFoundException("Pagamento não encontrado");

            pagamento.Confirmar();

            var pedido = _pedidoRepository.ObterPorId(pedidoId)
                ?? throw new NotFoundException("Pedido não encontrado");

            pedido.MarcarComoPago();

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public void CancelarPagamento(int pedidoId)
    {
        _logger.LogWarning(
            "Cancelando pagamento | PedidoId: {PedidoId}",
            pedidoId);

        var pagamento = _pagamentoRepository
            .ObterPagamentoPendentePorPedido(pedidoId)
            ?? throw new NotFoundException("Pagamento não encontrado");

        if (pagamento.Status != StatusPagamento.Pendente)
            throw new BusinessException("Pagamento não pode ser cancelado");

        pagamento.Cancelar();
        _pagamentoRepository.Atualizar(pagamento);

        _logger.LogInformation(
            "Pagamento cancelado | PagamentoId: {PagamentoId}",
            pagamento.Id);
    }

    public void ProcessarWebhookPix(
    string referencia,
    decimal valor,
    string status)
    {
        _logger.LogInformation(
            "Webhook PIX recebido | Referencia: {Referencia} | Valor: {Valor} | Status: {Status}",
            referencia, valor, status);

        if (status.ToLower() != "paid")
        {
            _logger.LogInformation(
                "Webhook ignorado | Status não pago | Referencia: {Referencia}",
                referencia);
            return;
        }

        var pagamento = _pagamentoRepository.ObterPorReferencia(referencia);

        if (pagamento == null)
        {
            _logger.LogWarning(
                "Webhook PIX ignorado | Pagamento inexistente | Referencia: {Referencia}",
                referencia);
            return; // 🔒 idempotência
        }

        if (pagamento.Status == StatusPagamento.Pago)
        {
            _logger.LogInformation(
                "Webhook PIX duplicado | Pagamento já confirmado | PagamentoId: {PagamentoId}",
                pagamento.Id);
            return; // 🔒 idempotência
        }

        if (Math.Abs(pagamento.Valor - valor) > 0.01m)
        {
            _logger.LogError("Divergência de valor PIX...");
            return;
        }

        pagamento.Confirmar();

        var pedido = _pedidoRepository.ObterPorId(pagamento.PedidoId)
            ?? throw new NotFoundException("Pedido não encontrado");

        pedido.MarcarComoPago();

        _pagamentoRepository.Atualizar(pagamento);
        _pedidoRepository.Atualizar(pedido);

        _logger.LogInformation(
            "Pagamento confirmado via PIX | PagamentoId: {PagamentoId} | PedidoId: {PedidoId}",
            pagamento.Id, pedido.Id);
    }
}
