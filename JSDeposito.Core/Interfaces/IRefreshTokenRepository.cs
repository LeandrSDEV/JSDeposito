using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface IRefreshTokenRepository
{
    void Salvar(RefreshToken token);
    RefreshToken? Obter(string token);
    void Revogar(string token);
}
