using JSDeposito.Core.Enums;
using JSDeposito.Core.ValueObjects;
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
    public EnderecoSnapshot? EnderecoEntrega { get; private set; }
    public bool FretePromocional { get; private set; }


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
        GarantirPedidoEditavel();

        var itemExistente = _itens.FirstOrDefault(i => i.ProdutoId == produto.Id);

        if (itemExistente != null)
            throw new Exception("Produto já adicionado ao pedido");

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
        ValidarParaPagamento();

        Status = PedidoStatus.Pago;
    }

    public void AplicarCupom(Cupom cupom)
    {
        GarantirPedidoEditavel();

        var desconto = cupom.CalcularDesconto(_itens.Sum(i => i.Subtotal));

        if (desconto <= 0)
            throw new Exception("Desconto inválido");

        Desconto = desconto;
        CodigoCupom = cupom.Codigo;

        RecalcularTotal();
    }

    public void AplicarFrete(decimal valorFrete, bool promocional = false)
    {
        GarantirPedidoEditavel();

        ValorFrete = valorFrete;
        FretePromocional = promocional;
        RecalcularTotal();
    }

    public bool EstaEmAberto()
    {
        return Status == PedidoStatus.Criado;
    }

    public ItemPedido RemoverItemPorProduto(int produtoId)
    {
        GarantirPedidoEditavel();

        var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);

        if (item == null)
            throw new Exception("Item não encontrado no pedido");

        _itens.Remove(item);
        RecalcularTotal();

        return item;
    }

    public void Cancelar()
    {
        if (Status != PedidoStatus.Criado)
            throw new Exception("Pedido pago não pode ser cancelado");

        Status = PedidoStatus.Cancelado;
    }

    public void ValidarParaPagamento()
    {
        if (Status == PedidoStatus.Cancelado)
            throw new Exception("Pedido cancelado não pode ser pago");

        if (Status != PedidoStatus.Criado)
            throw new Exception("Pedido não pode ser pago");

        if (!_itens.Any())
            throw new Exception("Pedido sem itens não pode ser pago");

        if (ValorFrete <= 0 && !FretePromocional)
            throw new Exception("Frete não calculado");

        if (EnderecoEntrega == null)
            throw new Exception("Endereço de entrega não informado");
    }

    private void GarantirPedidoEditavel()
    {
        if (Status != PedidoStatus.Criado)
            throw new Exception("Pedido não pode ser alterado");
    }

    public void DefinirEnderecoEntrega(EnderecoSnapshot endereco)
    {
        if (Status != PedidoStatus.Criado)
            throw new Exception("Endereço não pode ser alterado");

        EnderecoEntrega = endereco;
    }

    public void DefinirEnderecoEAplicarFrete(
    EnderecoSnapshot endereco,
    decimal valorFrete,
    bool promocional)
    {
        GarantirPedidoEditavel();

        EnderecoEntrega = endereco;

        ValorFrete = valorFrete;
        FretePromocional = promocional;

        RecalcularTotal();
    }
}
