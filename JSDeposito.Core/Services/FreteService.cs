namespace JSDeposito.Core.Services;

public class FreteService
{
    private readonly Localizacao _origem;

    public FreteService(Localizacao origem)
    {
        _origem = origem;
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
        if (distanciaKm <= 5) return 10m;
        if (distanciaKm <= 10) return 20m;
        return 30m + (decimal)(distanciaKm - 10) * 2m;
    }

    private double ToRadians(double valor)
        => valor * Math.PI / 180;
}


public record Localizacao(double Latitude, double Longitude);