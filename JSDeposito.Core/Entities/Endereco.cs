namespace JSDeposito.Core.Entities;

public class Endereco
{
    public int Id { get; private set; }
    public int ClienteId { get; private set; }

    public string Rua { get; private set; }
    public string Numero { get; private set; }
    public string Bairro { get; private set; }
    public string Cidade { get; private set; }

    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    protected Endereco() { }

    public Endereco(
        int clienteId,
        string rua,
        string numero,
        string bairro,
        string cidade,
        double latitude,
        double longitude)
    {
        ClienteId = clienteId;
        Rua = rua;
        Numero = numero;
        Bairro = bairro;
        Cidade = cidade;
        Latitude = latitude;
        Longitude = longitude;
    }
}
