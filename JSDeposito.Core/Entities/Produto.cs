namespace JSDeposito.Core.Entities;

public class Produto
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public decimal Preco { get; private set; }
    public int Estoque { get; private set; }
    public bool Ativo { get; private set; }

    protected Produto() { }

    public Produto(string nome, decimal preco, int estoqueInicial)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new Exception("Nome do produto inválido");

        if (preco <= 0)
            throw new Exception("Preço inválido");

        if (estoqueInicial < 0)
            throw new Exception("Estoque inválido");

        Nome = nome;
        Preco = preco;
        Estoque = estoqueInicial;
        Ativo = true;
    }

    public void AtualizarPreco(decimal preco)
    {
        if (preco <= 0)
            throw new Exception("Preço inválido");

        Preco = preco;
    }

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;

    public void EntradaEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new Exception("Quantidade inválida");

        Estoque += quantidade;
    }

    public void SaidaEstoque(int quantidade)
    {
        if (!Ativo)
            throw new Exception("Produto inativo");

        if (quantidade <= 0)
            throw new Exception("Quantidade inválida");

        if (Estoque < quantidade)
            throw new Exception("Estoque insuficiente");

        Estoque -= quantidade;
    }
}
