using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Api.Data;

public class PagamentoRepository : IPagamentoRepository
{
    private readonly AppDbContext _context;

    public PagamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Criar(Pagamento pagamento)
    {
        _context.Pagamentos.Add(pagamento);
        _context.SaveChanges();
    }

    public Pagamento ObterPorPedido(int pedidoId)
    {
        return _context.Pagamentos.First(p => p.PedidoId == pedidoId);
    }
}
