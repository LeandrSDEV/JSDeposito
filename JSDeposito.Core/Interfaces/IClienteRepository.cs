using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface IClienteRepository
{
    Cliente ObterPorId(int id);
    Cliente ObterPorEmail(string email);
    void Criar(Cliente cliente);
}
