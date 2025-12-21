using JSDeposito.Core.DTOs;
using JSDeposito.Core.Interfaces;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

public class NominatimGeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;

    public NominatimGeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "JSDeposito/1.0 (contato@seudominio.com)"
        );
    }

    public async Task<(double latitude, double longitude)> ObterCoordenadasAsync(string enderecoCompleto)
    {
        var url =
            $"search?q={Uri.EscapeDataString(enderecoCompleto)}" +
            $"&format=json&limit=1";

        var response = await _httpClient.GetFromJsonAsync<List<NominatimResponse>>(url);

        if (response == null || response.Count == 0)
            throw new Exception("Endereço não encontrado");

        return (response[0].Lat, response[0].Lon);
    }
}
