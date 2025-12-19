using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Core.Services;

public class PedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICupomRepository _cupomRepository;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProdutoRepository produtoRepository,
        ICupomRepository cupomRepository)
    {
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
        _cupomRepository = cupomRepository;
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
}
