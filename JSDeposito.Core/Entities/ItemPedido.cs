using JSDeposito.Core.Entities;

public class ItemPedido
{
    public int Id { get; private set; }
    public int ProdutoId { get; private set; }
    public string NomeProduto { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public int Quantidade { get; private set; }

    protected ItemPedido() { }

    public ItemPedido(Produto produto, int quantidade)
    {
        if (quantidade <= 0)
            throw new Exception("Quantidade inválida");

        ProdutoId = produto.Id;
        NomeProduto = produto.Nome;
        PrecoUnitario = produto.Preco;
        Quantidade = quantidade;
    }

    public decimal Subtotal => PrecoUnitario * Quantidade;

    public void AumentarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new Exception("Quantidade inválida");

        Quantidade += quantidade;
    }
}
