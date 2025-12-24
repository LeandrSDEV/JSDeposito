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
    public string? Referencia { get; private set; }

    protected Pagamento() { }

    public Pagamento(int pedidoId, decimal valor, TipoPagamento tipo)
    {
        PedidoId = pedidoId;
        Valor = valor;
        Tipo = tipo;
        Status = StatusPagamento.Pendente;
        CriadoEm = DateTime.UtcNow;

        Referencia = pedidoId.ToString();
    }

    public void Confirmar()
    {
        if (Status == StatusPagamento.Pago)
            return; // idempotência

        Status = StatusPagamento.Pago;
    }

    public void Cancelar()
    {
        if (Status == StatusPagamento.Pago)
            throw new Exception("Pagamento já foi confirmado");

        Status = StatusPagamento.Cancelado;
    }

    public void DefinirReferencia(string referencia)
    {
        Referencia = referencia;
    }

}
