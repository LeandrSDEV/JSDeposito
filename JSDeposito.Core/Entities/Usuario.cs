namespace JSDeposito.Core.Entities;

public class Usuario
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string Telefone { get; private set; }
    public string SenhaHash { get; private set; }
    public string Role { get; private set; }
    public List<Endereco> Enderecos { get; private set; } = new();

    protected Usuario() { }

    public Usuario(string nome, string email, string telefone, string senhaHash, string role)
    {
        Nome = nome;
        Email = email;
        Telefone = telefone;
        SenhaHash = senhaHash;
        Role = role;
    }

    public bool ValidarSenha(string senha)
    {
        return BCrypt.Net.BCrypt.Verify(senha, SenhaHash);
    }

    public void AdicionarEndereco(Endereco endereco)
    {
        Enderecos.Add(endereco);
    }

    public void AtualizarTelefone(string telefone)
    {
        Telefone = telefone;
    }
}
