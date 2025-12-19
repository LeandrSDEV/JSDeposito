namespace JSDeposito.Core.Entities;

public class ItemPedido
{
    public int Id { get; private set; }
    public int ProdutoId { get; private set; }
    public string NomeProduto { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public int Quantidade { get; private set; }

    public decimal Subtotal => PrecoUnitario * Quantidade;

    protected ItemPedido() { }

    public ItemPedido(int produtoId, string nomeProduto, decimal preco, int quantidade)
    {
        if (quantidade <= 0)
            throw new Exception("Quantidade inválida");

        ProdutoId = produtoId;
        NomeProduto = nomeProduto;
        PrecoUnitario = preco;
        Quantidade = quantidade;
    }
}
