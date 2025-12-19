using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Api.Data;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Produto ObterPorId(int id)
        => _context.Produtos.Find(id)!;

    public IEnumerable<Produto> ObterTodos()
        => _context.Produtos.ToList();

    public void Atualizar(Produto produto)
    {
        _context.Produtos.Update(produto);
        _context.SaveChanges();
    }
}
