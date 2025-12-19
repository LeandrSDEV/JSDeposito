namespace JSDeposito.Core.Entities;

public class Cliente
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string Telefone { get; private set; }

    public List<Endereco> Enderecos { get; private set; } = new();

    protected Cliente() { }

    public Cliente(string nome, string email, string telefone)
    {
        Nome = nome;
        Email = email;
        Telefone = telefone;
    }

    public void AdicionarEndereco(Endereco endereco)
    {
        Enderecos.Add(endereco);
    }
}
