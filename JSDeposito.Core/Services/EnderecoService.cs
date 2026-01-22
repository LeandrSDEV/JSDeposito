using Microsoft.Extensions.Logging;
using JSDeposito.Core.DTOs;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;
using JSDeposito.Core.Exceptions;

namespace JSDeposito.Core.Services;

public class EnderecoService
{
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IGeocodingService _geocodingService;
    private readonly ILogger<EnderecoService> _logger;

    public EnderecoService(
        IEnderecoRepository enderecoRepository,
        IUsuarioRepository usuarioRepository,
        IGeocodingService geocodingService,
        ILogger<EnderecoService> logger)
    {
        _enderecoRepository = enderecoRepository;
        _usuarioRepository = usuarioRepository;
        _geocodingService = geocodingService;
        _logger = logger;
    }

    public async Task CriarEndereco(EnderecoDto dto, int usuarioId)
    {
        _logger.LogInformation(
            "Iniciando criação de endereço | UsuarioId: {UsuarioId}",
            usuarioId);

        var usuario = _usuarioRepository.ObterPorId(usuarioId)
            ?? throw new Exception("Usuário não encontrado");

        var enderecoCompleto =
            $"{dto.Rua}, {dto.Numero}, {dto.Bairro}, {dto.Cidade}, Brasil";

        _logger.LogInformation(
            "Geocodificando endereço | UsuarioId: {UsuarioId} | Endereco: {Endereco}",
            usuarioId,
            enderecoCompleto);

        double lat = 0, lon = 0;

        try
        {
            (lat, lon) =
                await _geocodingService.ObterCoordenadasAsync(enderecoCompleto);

            _logger.LogInformation(
                "Geocoding concluído | Latitude: {Lat} | Longitude: {Lon}",
                lat,
                lon);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Falha ao geocodificar endereço | UsuarioId: {UsuarioId} | Endereco: {Endereco}",
                usuarioId,
                enderecoCompleto);
        }

        if (lat == 0 && lon == 0)
        {
            _logger.LogWarning(
                "Endereço salvo sem coordenadas | UsuarioId: {UsuarioId} | Endereco: {Endereco}",
                usuarioId,
                enderecoCompleto);
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

        _logger.LogInformation(
            "Endereço criado com sucesso | UsuarioId: {UsuarioId} | EnderecoId: {EnderecoId}",
            usuarioId,
            endereco.Id);
    }
    public List<Endereco> ListarAtivosPorUsuario(int usuarioId)
    {
        return _enderecoRepository.ObterAtivosPorUsuario(usuarioId);
    }

    public Endereco ObterPorIdDoUsuario(int enderecoId, int usuarioId)
    {
        var endereco = _enderecoRepository.ObterPorId(enderecoId)
            ?? throw new NotFoundException("Endereço não encontrado");

        if (!endereco.Ativo || endereco.UsuarioId != usuarioId)
            throw new BusinessException("Endereço inválido");

        return endereco;
    }

}
