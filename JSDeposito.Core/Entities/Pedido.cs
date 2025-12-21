using JSDeposito.Core.Enums;
using System.Collections.ObjectModel;
using System.Linq;

namespace JSDeposito.Core.Entities;

public class Pedido
{
    public int Id { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public PedidoStatus Status { get; private set; }
    public decimal Total { get; private set; }
    public decimal Desconto { get; private set; }
    public decimal ValorFrete { get; private set; }
    public string? CodigoCupom { get; private set; }

    private readonly List<ItemPedido> _itens = new();
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

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
        Total = _itens.Sum(i => i.Subtotal) - Desconto + ValorFrete;
    }

    public void MarcarComoPago()
    {
        if (Status != PedidoStatus.Criado)
            throw new Exception("Pedido não pode ser pago");

        Status = PedidoStatus.Pago;
    }

    public void AplicarCupom(Cupom cupom)
    {
        var desconto = cupom.CalcularDesconto(_itens.Sum(i => i.Subtotal));

        if (desconto <= 0)
            throw new Exception("Desconto inválido");

        Desconto = desconto;
        CodigoCupom = cupom.Codigo;

        RecalcularTotal();
    }

    public void AplicarFrete(decimal valorFrete)
    {
        ValorFrete = valorFrete;
        RecalcularTotal();
    }

    public bool EstaEmAberto()
    {
        return Status == PedidoStatus.Criado;
    }

    public void RemoverItemPorProduto(int produtoId)
    {
        var produto = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);

        if (produto == null)
            throw new Exception("Item não encontrado no pedido");

        _itens.Remove(produto);
        RecalcularTotal();
    }

}
