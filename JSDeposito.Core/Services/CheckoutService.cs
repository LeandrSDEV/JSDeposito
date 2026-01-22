using JSDeposito.Core.DTOs;
using JSDeposito.Core.Enums;
using JSDeposito.Core.Interfaces;
using JSDeposito.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Security;

namespace JSDeposito.Core.Services;

public class CheckoutService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly ICupomRepository _cupomRepository;
    private readonly FreteService _freteService;
    private readonly PagamentoService _pagamentoService;
    private readonly ILogger<CheckoutService> _logger;

    public CheckoutService(
        IPedidoRepository pedidoRepository,
        IEnderecoRepository enderecoRepository,
        ICupomRepository cupomRepository,
        FreteService freteService,
        PagamentoService pagamentoService,
        ILogger<CheckoutService> logger)
    {
        _pedidoRepository = pedidoRepository;
        _enderecoRepository = enderecoRepository;
        _cupomRepository = cupomRepository;
        _freteService = freteService;
        _pagamentoService = pagamentoService;
        _logger = logger;
    }

    public void RealizarCheckout(CheckoutDto dto, int usuarioId)
    {
        _logger.LogInformation(
            "Iniciando checkout | UsuarioId: {UsuarioId} | PedidoId: {PedidoId}",
            usuarioId,
            dto.PedidoId);

        var pedido = _pedidoRepository.ObterPorId(dto.PedidoId)
            ?? throw new Exception("Pedido não encontrado");

        if (pedido.UsuarioId != usuarioId)
        {
            _logger.LogWarning(
                "Tentativa de checkout não autorizada | UsuarioId: {UsuarioId} | PedidoId: {PedidoId}",
                usuarioId,
                pedido.Id);

            throw new SecurityException("Pedido não pertence ao usuário");
        }

        var endereco = _enderecoRepository.ObterPorId(dto.EnderecoId);
        if (endereco == null || endereco.UsuarioId != usuarioId)
            throw new Exception("Endereço inválido");

        _logger.LogInformation(
            "Endereço validado | PedidoId: {PedidoId} | EnderecoId: {EnderecoId}",
            pedido.Id,
            endereco.Id);

        if (!string.IsNullOrEmpty(dto.CodigoCupom))
        {
            _logger.LogInformation(
                "Aplicando cupom | PedidoId: {PedidoId} | Cupom: {Cupom}",
                pedido.Id,
                dto.CodigoCupom);

            var cupom = _cupomRepository.ObterPorCodigo(dto.CodigoCupom);
            pedido.AplicarCupom(cupom);
        }

        var destino = new Localizacao(endereco.Latitude, endereco.Longitude);
        var distancia = _freteService.CalcularDistanciaKm(destino);
        var valorFrete = _freteService.CalcularValorFrete(distancia);

        pedido.AplicarFrete(valorFrete);

        _logger.LogInformation(
            "Frete aplicado | PedidoId: {PedidoId} | DistanciaKm: {Distancia} | Valor: {ValorFrete}",
            pedido.Id,
            Math.Round(distancia, 2),
            valorFrete);

        _logger.LogInformation(
            "Criando pagamento | PedidoId: {PedidoId} | Tipo: {TipoPagamento}",
            pedido.Id,
            dto.TipoPagamento);

        _pagamentoService
            .CriarPagamento(pedido.Id, dto.TipoPagamento, usuarioId);

        _pedidoRepository.Atualizar(pedido);

        _logger.LogInformation(
            "Checkout concluído com sucesso | PedidoId: {PedidoId}",
            pedido.Id);
    }


    public object CriarCheckout(int pedidoId, TipoPagamento tipo, int usuarioId)
    {
        _logger.LogInformation(
            "Criando checkout rápido | PedidoId: {PedidoId} | UsuarioId: {UsuarioId}",
            pedidoId,
            usuarioId);

        return _pagamentoService.CriarPagamento(pedidoId, tipo, usuarioId);
    }

    public void Finalizar(int pedidoId, int usuarioId)
    {
        _logger.LogInformation(
            "Finalizando pedido | PedidoId: {PedidoId} | UsuarioId: {UsuarioId}",
            pedidoId,
            usuarioId);

        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new Exception("Pedido não encontrado");

        // Segurança: pedido deve pertencer ao usuário. Se ainda estiver anônimo, associa aqui.
        if (!pedido.UsuarioId.HasValue)
        {
            pedido.AssociarUsuario(usuarioId);
        }
        else if (pedido.UsuarioId != usuarioId)
        {
            throw new SecurityException("Pedido não pertence ao usuário");
        }

        // Valida se está pronto para pagamento (itens, endereço, frete etc.)
        pedido.ValidarParaPagamento();

        _pedidoRepository.Atualizar(pedido);

        _logger.LogInformation(
            "Pedido validado para pagamento | PedidoId: {PedidoId}",
            pedidoId);
    }

}
