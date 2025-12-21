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

    public void Criar(Produto produto)
    {
        _context.Produtos.Add(produto);
        _context.SaveChanges();
    }

    public void Atualizar(Produto produto)
    {
        _context.Produtos.Update(produto);
        _context.SaveChanges();
    }

    public Produto? ObterPorId(int id)
    {
        return _context.Produtos.Find(id);
    }

    public IEnumerable<Produto> ListarAtivos()
    {
        return _context.Produtos.Where(p => p.Ativo).ToList();
    }
}
