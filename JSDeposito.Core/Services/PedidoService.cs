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
        if (dto.Itens == null || !dto.Itens.Any())
            throw new Exception("Pedido deve conter ao menos um item");

        var pedido = new Pedido();

        foreach (var item in dto.Itens)
        {
            var produto = _produtoRepository.ObterPorId(item.ProdutoId);

            if (produto == null)
                throw new Exception($"Produto {item.ProdutoId} não encontrado");

            produto.BaixarEstoque(item.Quantidade);

            pedido.AdicionarItem(produto, item.Quantidade);
        }

        if (!string.IsNullOrWhiteSpace(dto.CodigoCupom))
        {
            var cupom = _cupomRepository.ObterPorCodigo(dto.CodigoCupom);

            if (cupom == null)
                throw new Exception("Cupom inválido ou inexistente");

            pedido.AplicarCupom(cupom);

            cupom.RegistrarUso();
            _cupomRepository.Atualizar(cupom);
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
        _pedidoRepository.Atualizar(pedido);
    }
    public Pedido ObterPedido(int pedidoId)
    {
        return _pedidoRepository.ObterPorId(pedidoId);
    }

    public void AdicionarItem(int pedidoId, AdicionarItemPedidoDto dto)
    {
        var pedido = _pedidoRepository.ObterPorId(pedidoId);
        if (pedido == null)
            throw new Exception("Pedido não encontrado");

        if (!pedido.EstaEmAberto())
            throw new Exception("Pedido não está em aberto");

        var produto = _produtoRepository.ObterPorId(dto.ProdutoId);
        if (produto == null)
            throw new Exception("Produto não encontrado");

        pedido.AdicionarItem(produto, dto.Quantidade);
        _pedidoRepository.Atualizar(pedido);
    }

    public void RemoverItemPorProduto(int pedidoId, int produtoId)
    {
        var pedido = _pedidoRepository.ObterPorId(pedidoId);

        if (pedido == null)
            throw new Exception("Pedido não encontrado");

        pedido.RemoverItemPorProduto(produtoId);

        _pedidoRepository.Atualizar(pedido);
    }

}
