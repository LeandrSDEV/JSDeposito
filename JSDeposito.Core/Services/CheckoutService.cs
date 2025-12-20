using JSDeposito.Core.DTOs;
using JSDeposito.Core.Interfaces;

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

    public void RealizarCheckout(CheckoutDto dto)
    {
        // 1️⃣ Pedido
        var pedido = _pedidoRepository.ObterPorId(dto.PedidoId);
        if (pedido == null)
            throw new Exception("Pedido não encontrado");

        // 2️⃣ Cliente
        var cliente = _clienteRepository.ObterPorId(dto.ClienteId);
        if (cliente == null)
            throw new Exception("Cliente inválido");

        // 3️⃣ Endereço
        var endereco = _enderecoRepository.ObterPorId(dto.EnderecoId);
        if (endereco == null || endereco.ClienteId != cliente.Id)
            throw new Exception("Endereço inválido");

        // 4️⃣ Cupom (opcional)
        if (!string.IsNullOrEmpty(dto.CodigoCupom))
        {
            var cupom = _cupomRepository.ObterPorCodigo(dto.CodigoCupom);
            pedido.AplicarCupom(cupom);
        }

        // 5️⃣ Frete
        var distancia = _freteService.CalcularDistanciaKm(
            endereco.Latitude,
            endereco.Longitude
        );

        var valorFrete = _freteService.CalcularValorFrete(distancia);
        pedido.AplicarFrete(valorFrete);

        // 6️⃣ Pagamento
        _pagamentoService.CriarPagamento(pedido.Id, dto.TipoPagamento);

        // 7️⃣ Persistir estado final
        _pedidoRepository.Atualizar(pedido);
    }
}
