using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Exceptions;
using JSDeposito.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace JSDeposito.Core.Services;

public class ProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(
        IProdutoRepository produtoRepository,
        ILogger<ProdutoService> logger)
    {
        _produtoRepository = produtoRepository;
        _logger = logger;
    }
    public Produto Criar(string nome, decimal preco, int estoqueInicial)
    {
        _logger.LogInformation(
            "Criando produto | Nome: {Nome} | Preço: {Preco} | EstoqueInicial: {Estoque}",
            nome, preco, estoqueInicial);

        var produto = new Produto(nome, preco, estoqueInicial);

        _produtoRepository.Criar(produto);

        _logger.LogInformation(
            "Produto criado | ProdutoId: {ProdutoId}",
            produto.Id);

        return produto;
    }

    public Produto Obter(int id)
    {
        var produto = _produtoRepository.ObterPorId(id)
            ?? throw new NotFoundException("Produto não encontrado");

        return produto;
    }

    public IEnumerable<Produto> Listar()
    {
        _logger.LogInformation("Listando produtos ativos");
        return _produtoRepository.ListarAtivos();
    }

    public void EntradaEstoque(int produtoId, int quantidade)
    {
        _logger.LogInformation(
            "Entrada de estoque | ProdutoId: {ProdutoId} | Quantidade: {Quantidade}",
            produtoId, quantidade);

        if (quantidade <= 0)
            throw new BusinessException("Quantidade inválida");

        var produto = Obter(produtoId);

        produto.EntradaEstoque(quantidade);
        _produtoRepository.Atualizar(produto);
    }

    public void SaidaEstoque(int produtoId, int quantidade)
    {
        _logger.LogInformation(
            "Saída de estoque | ProdutoId: {ProdutoId} | Quantidade: {Quantidade}",
            produtoId, quantidade);

        if (quantidade <= 0)
            throw new BusinessException("Quantidade inválida");

        var produto = Obter(produtoId);

        produto.SaidaEstoque(quantidade);
        _produtoRepository.Atualizar(produto);
    }

    public void AtualizarProduto(int produtoId, AtualizarProdutoDto dto)
    {
        _logger.LogInformation(
            "Atualizando produto | ProdutoId: {ProdutoId}",
            produtoId);

        var produto = _produtoRepository.ObterPorId(produtoId)
            ?? throw new NotFoundException("Produto não encontrado");

        produto.AlterarNome(dto.Nome);
        produto.AlterarPreco(dto.Preco);

        AjustarEstoque(produto, dto.Estoque);

        _produtoRepository.Atualizar(produto);

        _logger.LogInformation(
            "Produto atualizado com sucesso | ProdutoId: {ProdutoId}",
            produtoId);
    }

    private void AjustarEstoque(Produto produto, int estoqueDesejado)
    {
        var diferenca = estoqueDesejado - produto.Estoque;

        if (diferenca > 0)
            produto.EntradaEstoque(diferenca);
        else if (diferenca < 0)
            produto.SaidaEstoque(Math.Abs(diferenca));
    }
}
