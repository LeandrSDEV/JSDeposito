namespace JSDeposito.Core.Services;

public class FreteService
{
    // Coordenadas fixas do depósito
    private const double DepositoLatitude = -23.55052;
    private const double DepositoLongitude = -46.633308;

    public double CalcularDistanciaKm(double lat, double lon)
    {
        const double R = 6371; // raio da Terra em km

        var dLat = ToRadians(lat - DepositoLatitude);
        var dLon = ToRadians(lon - DepositoLongitude);

        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(DepositoLatitude)) *
            Math.Cos(ToRadians(lat)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    public decimal CalcularValorFrete(double distanciaKm)
    {
        if (distanciaKm <= 5)
            return 10m;

        if (distanciaKm <= 10)
            return 20m;

        return 30m + (decimal)(distanciaKm - 10) * 2m;
    }

    private double ToRadians(double valor)
    {
        return valor * Math.PI / 180;
    }
}
