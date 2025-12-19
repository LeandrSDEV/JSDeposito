using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Core.Services;

public class PedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICupomRepository _cupomRepository;
    private readonly FreteService _freteService;
    private readonly IEnderecoRepository _enderecoRepository;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProdutoRepository produtoRepository,
        ICupomRepository cupomRepository,
        FreteService freteService,
        IEnderecoRepository enderecoRepository)
    {
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
        _cupomRepository = cupomRepository;
        _freteService = freteService;
        _enderecoRepository = enderecoRepository;
    }

    public Pedido CriarPedido(CriarPedidoDto dto)
    {
        var pedido = new Pedido();

        foreach (var item in dto.Itens)
        {
            var produto = _produtoRepository.ObterPorId(item.ProdutoId);

            produto.BaixarEstoque(item.Quantidade);

            pedido.AdicionarItem(produto, item.Quantidade);

            if (!string.IsNullOrEmpty(dto.CodigoCupom))
            {
                var cupom = _cupomRepository.ObterPorCodigo(dto.CodigoCupom);

                pedido.AplicarCupom(cupom);

                cupom.RegistrarUso();
                _cupomRepository.Atualizar(cupom);
            }
        }

        _pedidoRepository.Criar(pedido);
        return pedido;
    }

    public void AplicarFrete(int pedidoId, int enderecoId)
    {
        var pedido = _pedidoRepository.ObterPorId(pedidoId);
        var endereco = _enderecoRepository.ObterPorId(enderecoId);

        var distancia = _freteService.CalcularDistanciaKm(
            endereco.Latitude,
            endereco.Longitude
        );

        var valorFrete = _freteService.CalcularValorFrete(distancia);

        pedido.AplicarFrete(valorFrete);
    }
}
