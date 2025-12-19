using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Core.Services;

public class ClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public Cliente CriarCliente(string nome, string email, string telefone)
    {
        if (_clienteRepository.ObterPorEmail(email) != null)
            throw new Exception("Email já cadastrado");

        var cliente = new Cliente(nome, email, telefone);
        _clienteRepository.Criar(cliente);

        return cliente;
    }
}
