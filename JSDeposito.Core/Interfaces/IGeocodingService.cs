namespace JSDeposito.Core.Interfaces;

public interface IGeocodingService
{
    Task<(double latitude, double longitude)> ObterCoordenadasAsync(string enderecoCompleto);
}
