using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Enums;
using JSDeposito.Core.Interfaces;
using JSDeposito.Core.ValueObjects;

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

    public Pedido Criar(CriarPedidoDto dto)
    {
        var pedido = new Pedido();

        foreach (var item in dto.Itens)
        {
            var produto = _produtoRepository.ObterPorId(item.ProdutoId)
                ?? throw new Exception("Produto não encontrado");

            pedido.AdicionarItem(produto, item.Quantidade);
        }

        _pedidoRepository.Criar(pedido);
        return pedido;
    }

    public void AplicarFrete(int pedidoId, int enderecoId)
    {
        var pedido = _pedidoRepository.ObterPorId(pedidoId);
        var endereco = _enderecoRepository.ObterPorId(enderecoId);

        if (pedido == null || endereco == null)
            throw new Exception("Pedido ou endereço inválido");

        var snapshot = new EnderecoSnapshot(
            endereco.Rua,
            endereco.Numero,
            endereco.Bairro,
            endereco.Cidade,
            endereco.Latitude,
            endereco.Longitude
        );

        var (valorFrete, promocional) =
            _freteService.CalcularFrete(
                new Localizacao(snapshot.Latitude, snapshot.Longitude)
            );

        pedido.DefinirEnderecoEAplicarFrete(
            snapshot,
            valorFrete,
            promocional
        );

        _pedidoRepository.Atualizar(pedido);
    }

    public Pedido ObterPedido(int pedidoId)
    {
        return _pedidoRepository.ObterPorId(pedidoId);
    }

    public void AdicionarItem(int pedidoId, AdicionarItemPedidoDto dto)
    {
        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new Exception("Pedido não encontrado");

        if (pedido.Status != PedidoStatus.Criado)
            throw new Exception("Pedido não pode ser alterado");

        var produto = _produtoRepository.ObterPorId(dto.ProdutoId)
            ?? throw new Exception("Produto não encontrado");

        // 🔥 REGRA CRÍTICA
        pedido.AdicionarItem(produto, dto.Quantidade);

        _produtoRepository.Atualizar(produto);
        _pedidoRepository.Atualizar(pedido);
    }

    public void RemoverItemPorProduto(int pedidoId, int produtoId)
    {
        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new Exception("Pedido não encontrado");

        if (pedido.Status != PedidoStatus.Criado)
            throw new Exception("Pedido não pode ser alterado");

        // Remove do pedido e recupera o item
        var itemRemovido = pedido.RemoverItemPorProduto(produtoId);

        // Devolve estoque
        var produto = _produtoRepository.ObterPorId(produtoId)
            ?? throw new Exception("Produto não encontrado");

        produto.EntradaEstoque(itemRemovido.Quantidade);

        _produtoRepository.Atualizar(produto);
        _pedidoRepository.Atualizar(pedido);
    }

    public void CancelarPedido(int pedidoId)
    {
        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new Exception("Pedido não encontrado");

        if (pedido.Status != PedidoStatus.Criado)
            throw new Exception("Pedido não pode ser cancelado");

        foreach (var item in pedido.Itens)
        {
            var produto = _produtoRepository.ObterPorId(item.ProdutoId)
                ?? throw new Exception($"Produto {item.ProdutoId} não encontrado");

            produto.EntradaEstoque(item.Quantidade);
            _produtoRepository.Atualizar(produto);
        }

        pedido.Cancelar();
        _pedidoRepository.Atualizar(pedido);
    }

}
