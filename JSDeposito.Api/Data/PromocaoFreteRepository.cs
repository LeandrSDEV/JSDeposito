using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JSDeposito.Api.Data;


public class PromocaoFreteRepository : IPromocaoFreteRepository
{
    private readonly AppDbContext _context;

    public PromocaoFreteRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Criar(PromocaoFrete promocao)
    {
        _context.PromocaoFretes.Add(promocao);
        _context.SaveChanges();
    }

    public PromocaoFrete? ObterPorId(int id)
    {
        return _context.PromocaoFretes.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<PromocaoFrete> Listar()
    {
        return _context.PromocaoFretes
            .OrderByDescending(p => p.Inicio)
            .ToList();
    }

    public void Atualizar(PromocaoFrete promocao)
    {
        _context.PromocaoFretes.Update(promocao);
        _context.SaveChanges();
    }

    public PromocaoFrete? ObterPromocaoAtiva()
    {
        var agora = DateTime.Now;

        return _context.PromocaoFretes
            .AsNoTracking()
            .FirstOrDefault(p =>
                p.Ativa &&
                agora >= p.Inicio &&
                agora <= p.Fim
            );
    }
}