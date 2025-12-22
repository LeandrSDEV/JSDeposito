namespace JSDeposito.Core.Entities;

public class RefreshToken
{
    public int Id { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiraEm { get; private set; }
    public bool Revogado { get; private set; }
    public int UsuarioId { get; private set; }

    protected RefreshToken() { }

    public RefreshToken(int usuarioId, string token, DateTime expiraEm)
    {
        UsuarioId = usuarioId;
        Token = token;
        ExpiraEm = expiraEm;
        Revogado = false;
    }

    public bool EstaValido()
        => !Revogado && DateTime.UtcNow <= ExpiraEm;

    public void Revogar()
        => Revogado = true;
}
