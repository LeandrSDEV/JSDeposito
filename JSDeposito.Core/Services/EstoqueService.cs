using JSDeposito.Core.Interfaces;

namespace JSDeposito.Core.Services;

public class EstoqueService
{
    private readonly IProdutoRepository _produtoRepository;

    public EstoqueService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public void Baixar(int produtoId, int quantidade)
    {
        var produto = _produtoRepository.ObterPorId(produtoId);
        produto.BaixarEstoque(quantidade);
        _produtoRepository.Atualizar(produto);
    }
}
