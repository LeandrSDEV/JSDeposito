namespace JSDeposito.Core.DTOs;

public class AplicarFreteLocalizacaoDto
{
    public string Rua { get; set; } = "";
    public string Numero { get; set; } = "";
    public string Bairro { get; set; } = "";
    public string Cidade { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
