using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Api.Data;

public class CupomRepository : ICupomRepository
{
    private readonly AppDbContext _context;

    public CupomRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Criar(Cupom cupom)
    {
        _context.Cupons.Add(cupom);
        _context.SaveChanges();
    }

    public Cupom ObterPorCodigo(string codigo)
    {
        return _context.Cupons.FirstOrDefault(c => c.Codigo == codigo);
    }

    public void Atualizar(Cupom cupom)
    {
        _context.Cupons.Update(cupom);
        _context.SaveChanges();
    }


}
