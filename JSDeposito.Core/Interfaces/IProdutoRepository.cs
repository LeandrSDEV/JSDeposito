using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface IProdutoRepository
{
    void Criar(Produto produto);
    void Atualizar(Produto produto);
    Produto? ObterPorId(int id);
    IEnumerable<Produto> ListarAtivos();
}
