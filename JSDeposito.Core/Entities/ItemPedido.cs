using JSDeposito.Core.Exceptions;

namespace JSDeposito.Core.Entities;


public class ItemPedido
{
    public int Id { get; private set; }
    public int ProdutoId { get; private set; }
    public Produto Produto { get; private set; } = null!;
    public string NomeProduto { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public int Quantidade { get; private set; }

    protected ItemPedido() { }

    public ItemPedido(Produto produto, int quantidade)
    {
        if (quantidade <= 0)
            throw new Exception("Quantidade inválida");

        Produto = produto;
        ProdutoId = produto.Id;
        NomeProduto = produto.Nome;
        PrecoUnitario = produto.Preco;
        Quantidade = quantidade;
    }

    public decimal Subtotal => PrecoUnitario * Quantidade;

    public void AlterarQuantidade(int novaQuantidade)
    {
        if (novaQuantidade <= 0)
            throw new BusinessException("Quantidade inválida");

        Quantidade = novaQuantidade;
    }

    public void AumentarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new Exception("Quantidade inválida");

        Quantidade += quantidade;
    }
}