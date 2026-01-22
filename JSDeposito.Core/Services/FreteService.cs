using Microsoft.Extensions.Logging;
using JSDeposito.Core.Interfaces;
using JSDeposito.Core.ValueObjects;

namespace JSDeposito.Core.Services;


public class FreteService
{
    private readonly Localizacao _origem;
    private readonly IPromocaoFreteRepository _promocaoRepository;
    private readonly ILogger<FreteService> _logger;

    public FreteService(
        Localizacao origem,
        IPromocaoFreteRepository promocaoRepository,
        ILogger<FreteService> logger)
    {
        _origem = origem;
        _promocaoRepository = promocaoRepository;
        _logger = logger;
    }

    public (decimal valor, bool promocional) CalcularFrete(Localizacao destino)
    {
        _logger.LogInformation(
            "Calculando frete | Origem: ({OrigLat},{OrigLon}) | Destino: ({DestLat},{DestLon})",
            _origem.Latitude,
            _origem.Longitude,
            destino.Latitude,
            destino.Longitude);

        var promocao = _promocaoRepository.ObterPromocaoAtiva();

        if (promocao != null && promocao.EstaAtiva())
        {
            _logger.LogInformation("Frete promocional aplicado | Valor: 0");
            return (0m, true);
        }

        var distancia = CalcularDistanciaKm(destino);
        var valor = CalcularValorFrete(distancia);

        _logger.LogInformation(
            "Frete calculado | Distância: {DistanciaKm} km | Valor: {Valor}",
            Math.Round(distancia, 2),
            valor);

        return (valor, false);
    }

    public double CalcularDistanciaKm(Localizacao destino)
    {
        const double R = 6371;
        var dLat = ToRadians(destino.Latitude - _origem.Latitude);
        var dLon = ToRadians(destino.Longitude - _origem.Longitude);

        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(_origem.Latitude)) *
            Math.Cos(ToRadians(destino.Latitude)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    public decimal CalcularValorFrete(double distanciaKm)
    {
        if (distanciaKm <= 5) return 5m;
        if (distanciaKm <= 10) return 10m;
        return 20m + (decimal)(distanciaKm - 10) * 2m;
    }

    private double ToRadians(double valor)
        => valor * Math.PI / 180;
}