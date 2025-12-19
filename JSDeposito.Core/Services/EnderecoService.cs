using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Core.Services;

public class EnderecoService
{
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly IClienteRepository _clienteRepository;

    public EnderecoService(
        IEnderecoRepository enderecoRepository,
        IClienteRepository clienteRepository)
    {
        _enderecoRepository = enderecoRepository;
        _clienteRepository = clienteRepository;
    }

    public Endereco CriarEndereco(
        int clienteId,
        string rua,
        string numero,
        string bairro,
        string cidade,
        double latitude,
        double longitude)
    {
        var cliente = _clienteRepository.ObterPorId(clienteId);

        if (cliente == null)
            throw new Exception("Cliente não encontrado");

        var endereco = new Endereco(
            clienteId,
            rua,
            numero,
            bairro,
            cidade,
            latitude,
            longitude
        );

        _enderecoRepository.Criar(endereco);
        return endereco;
    }
}
