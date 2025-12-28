using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Enums;
using JSDeposito.Core.Exceptions;
using JSDeposito.Core.Interfaces;
using JSDeposito.Core.ValueObjects;
using Microsoft.Extensions.Logging;

namespace JSDeposito.Core.Services;

public class PedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICupomRepository _cupomRepository;
    private readonly FreteService _freteService;
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProdutoRepository produtoRepository,
        ICupomRepository cupomRepository,
        FreteService freteService,
        IEnderecoRepository enderecoRepository,
        ILogger<PedidoService> logger)
    {
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
        _cupomRepository = cupomRepository;
        _freteService = freteService;
        _enderecoRepository = enderecoRepository;
        _logger = logger;
    }

    public CriarPedidoResponseDto CriarPedidoAnonimo(CriarPedidoDto dto)
    {
        _logger.LogInformation("Criando pedido anônimo");

        var pedido = new Pedido();
        pedido.GerarTokenAnonimo();

        foreach (var item in dto.Itens)
        {
            var produto = _produtoRepository.ObterPorId(item.ProdutoId)
                ?? throw new NotFoundException("Produto não encontrado");

            pedido.AdicionarItem(produto, item.Quantidade);
        }

        _pedidoRepository.Criar(pedido);

        _logger.LogInformation(
            "Pedido anônimo criado | PedidoId: {PedidoId}",
            pedido.Id);

        return new CriarPedidoResponseDto
        {
            PedidoId = pedido.Id,
            TokenAnonimo = pedido.TokenAnonimo!.Value
        };
    }

    public void AplicarFrete(int pedidoId, int enderecoId)
    {
        _logger.LogInformation(
            "Aplicando frete | PedidoId: {PedidoId} | EnderecoId: {EnderecoId}",
            pedidoId, enderecoId);

        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new NotFoundException("Pedido não encontrado");

        var endereco = _enderecoRepository.ObterPorId(enderecoId)
            ?? throw new NotFoundException("Endereço não encontrado");

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

        pedido.DefinirEnderecoEAplicarFrete(snapshot, valorFrete, promocional);
        _pedidoRepository.Atualizar(pedido);

        _logger.LogInformation(
            "Frete aplicado | PedidoId: {PedidoId} | Valor: {ValorFrete}",
            pedidoId, valorFrete);
    }

    public Pedido ObterPedido(int pedidoId)
    {
        return _pedidoRepository.ObterPorId(pedidoId);
    }

    public void AdicionarItem(int pedidoId, AdicionarItemPedidoDto dto)
    {
        _logger.LogInformation(
            "Adicionando item | PedidoId: {PedidoId} | ProdutoId: {ProdutoId}",
            pedidoId, dto.ProdutoId);

        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new NotFoundException("Pedido não encontrado");

        if (pedido.Status != PedidoStatus.Criado)
            throw new BusinessException("Pedido não pode ser alterado");

        var produto = _produtoRepository.ObterPorId(dto.ProdutoId)
            ?? throw new NotFoundException("Produto não encontrado");

        // REGRA DE ESTOQUE duplicada para garantir consistência
        if (produto.Estoque < dto.Quantidade)
            throw new BusinessException(
                $"Estoque atual: {produto.Estoque}"
            );

        if (produto.Estoque < dto.Quantidade)
            throw new BusinessException(
                $"Estoque atual: {produto.Estoque}"
            );

        pedido.AdicionarItem(produto, dto.Quantidade);

        _produtoRepository.Atualizar(produto);
        _pedidoRepository.Atualizar(pedido);

        _logger.LogInformation(
            "Item adicionado | PedidoId: {PedidoId} | ProdutoId: {ProdutoId}",
            pedidoId, dto.ProdutoId);
    }


    public void RemoverItemPorProduto(int pedidoId, int produtoId)
    {
        _logger.LogInformation(
            "Removendo produto {ProdutoId} do pedido {PedidoId}",
            produtoId,
            pedidoId
        );
        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new NotFoundException("Pedido não encontrado");

        if (pedido.Status != PedidoStatus.Criado)
            throw new BusinessException("Pedido não pode ser alterado");

        // Remove do pedido e recupera o item
        var itemRemovido = pedido.RemoverItemPorProduto(produtoId);

        // Devolve estoque
        var produto = _produtoRepository.ObterPorId(produtoId)
            ?? throw new NotFoundException("Produto não encontrado");

        produto.EntradaEstoque(itemRemovido.Quantidade);

        _produtoRepository.Atualizar(produto);
        _pedidoRepository.Atualizar(pedido);

        _logger.LogInformation(
            "Produto {ProdutoId} removido com sucesso do pedido {PedidoId}",
            produtoId,
            pedidoId
        );
    }

    public void CancelarPedido(int pedidoId)
    {
        _logger.LogWarning(
            "Cancelando pedido | PedidoId: {PedidoId}",
            pedidoId);

        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new NotFoundException("Pedido não encontrado");

        if (pedido.Status != PedidoStatus.Criado)
            throw new BusinessException("Pedido não pode ser cancelado");

        foreach (var item in pedido.Itens)
        {
            var produto = _produtoRepository.ObterPorId(item.ProdutoId)
                ?? throw new NotFoundException($"Produto {item.ProdutoId} não encontrado");

            produto.EntradaEstoque(item.Quantidade);
            _produtoRepository.Atualizar(produto);
        }

        pedido.Cancelar();
        _pedidoRepository.Atualizar(pedido);

        _logger.LogInformation(
            "Pedido cancelado com sucesso | PedidoId: {PedidoId}",
            pedidoId);
    }

    public void AlterarQuantidade(int pedidoId, int produtoId, int quantidade)
    {
        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new NotFoundException("Pedido não encontrado");

        if (pedido.Status != PedidoStatus.Criado)
            throw new BusinessException("Pedido não pode ser alterado");

        var item = pedido.Itens.FirstOrDefault(i => i.ProdutoId == produtoId)
            ?? throw new NotFoundException("Item não encontrado no pedido");

        // 🔒 valida estoque
        if (quantidade > item.Produto.Estoque)
            throw new BusinessException(
                $"Estoque atual: {item.Produto.Estoque}");

        if (quantidade <= 0)
            pedido.RemoverItemPorProduto(produtoId);
        else
            item.AlterarQuantidade(quantidade);

        _pedidoRepository.Atualizar(pedido);
    }

    public void AssociarPedidoAnonimoAoUsuario(Guid tokenAnonimo, int usuarioId)
    {
        var pedido = _pedidoRepository.ObterPorTokenAnonimo(tokenAnonimo)
            ?? throw new NotFoundException("Pedido não encontrado");

        if (pedido.UsuarioId != null)
            return;

        if (pedido.Status != PedidoStatus.Criado)
            throw new BusinessException("Pedido inválido");

        pedido.AssociarUsuario(usuarioId);
        pedido.RemoverTokenAnonimo();

        _pedidoRepository.Atualizar(pedido);
    }

    public void AplicarCupom(int pedidoId, string codigoCupom)
    {
        _logger.LogInformation(
    "Aplicando cupom {CodigoCupom} no pedido {PedidoId}",
    codigoCupom,
    pedidoId
);

        var pedido = _pedidoRepository.ObterPorId(pedidoId)
            ?? throw new NotFoundException("Pedido não encontrado");

        if (pedido.Status != PedidoStatus.Criado)
            throw new BusinessException("Pedido não pode receber cupom");

        var cupom = _cupomRepository.ObterPorCodigo(codigoCupom)
            ?? throw new BusinessException("Cupom inválido");

        pedido.AplicarCupom(cupom);

        _cupomRepository.Atualizar(cupom);
        _pedidoRepository.Atualizar(pedido);

        _logger.LogInformation(
   "Aplicado cupom {CodigoCupom} no pedido {PedidoId}",
   codigoCupom,
   pedidoId);

    }



}
