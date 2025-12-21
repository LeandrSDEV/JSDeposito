using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Core.Services;

public class ProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public Produto Criar(string nome, decimal preco, int estoqueInicial)
    {
        var produto = new Produto(nome, preco, estoqueInicial);

        _produtoRepository.Criar(produto);
        return produto;
    }

    public Produto Obter(int id)
    {
        var produto = _produtoRepository.ObterPorId(id);

        if (produto == null)
            throw new Exception("Produto não encontrado");

        return produto;
    }

    public IEnumerable<Produto> Listar()
    {
        return _produtoRepository.ListarAtivos();
    }

    public void EntradaEstoque(int produtoId, int quantidade)
    {
        var produto = Obter(produtoId);

        produto.EntradaEstoque(quantidade);
        _produtoRepository.Atualizar(produto);
    }

    public void SaidaEstoque(int produtoId, int quantidade)
    {
        var produto = Obter(produtoId);

        produto.SaidaEstoque(quantidade);
        _produtoRepository.Atualizar(produto);
    }

    public void AtualizarProduto(int produtoId, AtualizarProdutoDto dto)
    {
        var produto = _produtoRepository.ObterPorId(produtoId)
            ?? throw new Exception("Produto não encontrado");

        produto.AlterarNome(dto.Nome);
        produto.AlterarPreco(dto.Preco);

        AjustarEstoque(produto, dto.Estoque);

        _produtoRepository.Atualizar(produto);
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
