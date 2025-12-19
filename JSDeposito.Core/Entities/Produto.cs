namespace JSDeposito.Core.Entities;

public class Produto
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public decimal Preco { get; private set; }
    public int Estoque { get; private set; }

    protected Produto() { }

    public Produto(string nome, decimal preco, int estoque)
    {
        Nome = nome;
        Preco = preco;
        Estoque = estoque;
    }

    public void BaixarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new Exception("Quantidade inválida");

        if (quantidade > Estoque)
            throw new Exception("Estoque insuficiente");

        Estoque -= quantidade;
    }
}
