using JSDeposito.Core.DTOs;
using JSDeposito.Core.Exceptions;
using JSDeposito.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

public class NominatimGeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NominatimGeocodingService> _logger;

    public NominatimGeocodingService(
        HttpClient httpClient,
        ILogger<NominatimGeocodingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _httpClient.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "JSDeposito/1.0 (contato@seudominio.com)"
        );
    }

    public async Task<(double latitude, double longitude)> ObterCoordenadasAsync(
    string enderecoCompleto)
    {
        _logger.LogInformation(
            "Buscando coordenadas geográficas | Endereço: {Endereco}",
            enderecoCompleto);

        if (string.IsNullOrWhiteSpace(enderecoCompleto))
            throw new BusinessException("Endereço inválido para geocodificação");

        try
        {
            var url =
                $"search?q={Uri.EscapeDataString(enderecoCompleto)}" +
                $"&format=json&limit=1";

            var response =
                await _httpClient.GetFromJsonAsync<List<NominatimResponse>>(url);

            if (response == null || response.Count == 0)
            {
                _logger.LogWarning(
                    "Nenhuma coordenada encontrada | Endereço: {Endereco}",
                    enderecoCompleto);

                return (0, 0);
            }

            _logger.LogInformation(
                "Coordenadas encontradas | Lat: {Lat} | Lon: {Lon}",
                response[0].Lat, response[0].Lon);

            return (response[0].Lat, response[0].Lon);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(
                ex,
                "Timeout ao consultar Nominatim | Endereço: {Endereco}",
                enderecoCompleto);

            return (0, 0);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "Erro HTTP ao consultar Nominatim | Endereço: {Endereco}",
                enderecoCompleto);

            return (0, 0);
        }
    }
}
