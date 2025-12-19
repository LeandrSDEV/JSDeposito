using JSDeposito.Core.Enums;

namespace JSDeposito.Core.Entities;

public class Pagamento
{
    public int Id { get; private set; }
    public int PedidoId { get; private set; }
    public decimal Valor { get; private set; }
    public TipoPagamento Tipo { get; private set; }
    public StatusPagamento Status { get; private set; }
    public DateTime CriadoEm { get; private set; }

    protected Pagamento() { }

    public Pagamento(int pedidoId, decimal valor, TipoPagamento tipo)
    {
        if (valor <= 0)
            throw new Exception("Valor de pagamento inválido");

        PedidoId = pedidoId;
        Valor = valor;
        Tipo = tipo;
        Status = StatusPagamento.Pendente;
        CriadoEm = DateTime.Now;
    }

    public void Confirmar()
    {
        Status = StatusPagamento.Pago;
    }

    public void Cancelar()
    {
        Status = StatusPagamento.Cancelado;
    }
}
