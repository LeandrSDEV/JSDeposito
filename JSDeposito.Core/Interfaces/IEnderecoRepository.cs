using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface IEnderecoRepository
{
    Endereco? ObterPorId(int id);
    List<Endereco> ObterAtivosPorCliente(int clienteId);
    void Criar(Endereco endereco);
}
