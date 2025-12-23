using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface IEnderecoRepository
{
    Endereco? ObterPorId(int id);
    List<Endereco> ObterAtivosPorUsuario(int usuarioId);
    void Criar(Endereco endereco);
}
