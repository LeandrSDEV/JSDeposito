using JSDeposito.Core.Entities;
using JSDeposito.Core.Enums;
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

    public Pagamento? ObterPagamentoPendentePorPedido(int pedidoId)
    {
        return _context.Pagamentos.FirstOrDefault(p =>
            p.PedidoId == pedidoId &&
            p.Status == StatusPagamento.Pendente);
    }

    public void Atualizar(Pagamento pagamento)
    {
        _context.Pagamentos.Update(pagamento);
        _context.SaveChanges();
    }

    public Pagamento? ObterPorReferencia(string referencia)
    {
        return _context.Pagamentos
            .FirstOrDefault(p => p.Referencia == referencia);
    }
}
