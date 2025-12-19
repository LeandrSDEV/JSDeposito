using System.Collections.ObjectModel;

namespace JSDeposito.Core.Entities;

public class Pedido
{
    public int Id { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public PedidoStatus Status { get; private set; }
    public decimal Total { get; private set; }

    private readonly List<ItemPedido> _itens = new();
    public IReadOnlyCollection<ItemPedido> Itens => new ReadOnlyCollection<ItemPedido>(_itens);

    public Pedido()
    {
        DataCriacao = DateTime.Now;
        Status = PedidoStatus.Criado;
        Total = 0;
    }

    public void AdicionarItem(Produto produto, int quantidade)
    {
        var item = new ItemPedido(
            produto.Id,
            produto.Nome,
            produto.Preco,
            quantidade
        );

        _itens.Add(item);
        RecalcularTotal();
    }

    private void RecalcularTotal()
    {
        Total = _itens.Sum(i => i.Subtotal);
    }

    public void MarcarComoPago()
    {
        Status = PedidoStatus.Pago;
    }
}
