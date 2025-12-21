using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Core.Services;

public class EnderecoService
{
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IGeocodingService _geocodingService;


    public EnderecoService(
        IEnderecoRepository enderecoRepository,
        IClienteRepository clienteRepository,
        IGeocodingService geocodingService)
    {
        _enderecoRepository = enderecoRepository;
        _clienteRepository = clienteRepository;
        _geocodingService = geocodingService;
    }

    public async Task CriarEndereco(EnderecoDto dto)
    {
        var enderecoCompleto =
            $"{dto.Rua}, {dto.Numero}, {dto.Bairro}, {dto.Cidade}, Brasil";

        var (lat, lon) =
            await _geocodingService.ObterCoordenadasAsync(enderecoCompleto);

        var endereco = new Endereco(
            dto.ClienteId,
            dto.Rua,
            dto.Numero,
            dto.Bairro,
            dto.Cidade,
            lat,
            lon
        );

        _enderecoRepository.Criar(endereco);
    }
}
