using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Core.Services;

public class PedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoRepository _produtoRepository;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProdutoRepository produtoRepository)
    {
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
    }

    public Pedido CriarPedido(CriarPedidoDto dto)
    {
        var pedido = new Pedido();

        foreach (var item in dto.Itens)
        {
            var produto = _produtoRepository.ObterPorId(item.ProdutoId);

            produto.BaixarEstoque(item.Quantidade);

            pedido.AdicionarItem(produto, item.Quantidade);
        }

        _pedidoRepository.Criar(pedido);
        return pedido;
    }
}
