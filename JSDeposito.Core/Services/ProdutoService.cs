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
}
