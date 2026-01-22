using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface IUsuarioRepository
{
    Usuario? ObterPorEmail(string email);
    Usuario? ObterPorId(int id);
    void Adicionar(Usuario usuario);
}
