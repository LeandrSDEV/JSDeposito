using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Core.Services;

public class EnderecoService
{
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IGeocodingService _geocodingService;


    public EnderecoService(
        IEnderecoRepository enderecoRepository,
        IUsuarioRepository usuarioRepository,
        IGeocodingService geocodingService)
    {
        _enderecoRepository = enderecoRepository;
        _usuarioRepository = usuarioRepository;
        _geocodingService = geocodingService;
    }

    public async Task CriarEndereco(EnderecoDto dto, int usuarioId )
    {
        var usuario = _usuarioRepository.ObterPorId(usuarioId)
            ?? throw new Exception("Usuário não encontrado");

        var enderecoCompleto =
            $"{dto.Rua}, {dto.Numero}, {dto.Bairro}, {dto.Cidade}, Brasil";

        double lat = 0, lon = 0;

        try
        {
            (lat, lon) =
                await _geocodingService.ObterCoordenadasAsync(enderecoCompleto);
        }
        catch
        {
            // log apenas
        }

        var endereco = new Endereco(
            usuarioId,
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
