using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Enums;
using JSDeposito.Core.Interfaces;
using JSDeposito.Core.ValueObjects;
using System.Security;

namespace JSDeposito.Core.Services;

public class CheckoutService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly ICupomRepository _cupomRepository;
    private readonly FreteService _freteService;
    private readonly PagamentoService _pagamentoService;

    public CheckoutService(
        IPedidoRepository pedidoRepository,
        IClienteRepository clienteRepository,
        IEnderecoRepository enderecoRepository,
        ICupomRepository cupomRepository,
        FreteService freteService,
        PagamentoService pagamentoService)
    {
        _pedidoRepository = pedidoRepository;
        _clienteRepository = clienteRepository;
        _enderecoRepository = enderecoRepository;
        _cupomRepository = cupomRepository;
        _freteService = freteService;
        _pagamentoService = pagamentoService;
    }

    public void RealizarCheckout(CheckoutDto dto, int usuarioId)
    {
        var pedido = _pedidoRepository.ObterPorId(dto.PedidoId)
            ?? throw new Exception("Pedido não encontrado");

        if (pedido.UsuarioId != usuarioId)
            throw new SecurityException("Pedido não pertence ao usuário");

        var cliente = _clienteRepository.ObterPorId(usuarioId)
            ?? throw new Exception("Cliente não encontrado");

        var endereco = _enderecoRepository.ObterPorId(dto.EnderecoId);
        if (endereco == null || endereco.ClienteId != cliente.Id)
            throw new Exception("Endereço inválido");

        if (!string.IsNullOrEmpty(dto.CodigoCupom))
        {
            var cupom = _cupomRepository.ObterPorCodigo(dto.CodigoCupom);
            pedido.AplicarCupom(cupom);
        }

        var destino = new Localizacao(endereco.Latitude, endereco.Longitude);
        var distancia = _freteService.CalcularDistanciaKm(destino);
        var valorFrete = _freteService.CalcularValorFrete(distancia);

        pedido.AplicarFrete(valorFrete);

        _pagamentoService
            .CriarPagamento(pedido.Id, dto.TipoPagamento, usuarioId);

        _pedidoRepository.Atualizar(pedido);
    }


    public object CriarCheckout(int pedidoId, TipoPagamento tipo, int usuarioId)
    {
        return _pagamentoService.CriarPagamento(pedidoId, tipo, usuarioId);
    }

    public void Finalizar(int pedidoId, int usuarioId)
    {
        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new Exception("Pedido não encontrado");

        if (!pedido.EstaEmAberto())
            throw new Exception("Pedido não pode ser finalizado");

        pedido.AssociarUsuario(usuarioId);

        _pedidoRepository.Atualizar(pedido);
    }
}
