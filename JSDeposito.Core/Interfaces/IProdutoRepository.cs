using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface IProdutoRepository
{
    Produto ObterPorId(int id);
    IEnumerable<Produto> ObterTodos();
    void Atualizar(Produto produto);
}
