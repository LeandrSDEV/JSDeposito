namespace JSDeposito.Core.Entities;

public class Usuario
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string SenhaHash { get; private set; }
    public string Role { get; private set; }

    protected Usuario() { }

    public Usuario(string nome, string email, string senhaHash, string role)
    {
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        Role = role;
    }
}
