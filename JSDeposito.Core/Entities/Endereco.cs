namespace JSDeposito.Core.Entities;

public class Endereco
{
    public int Id { get; private set; }
    public int UsuarioId { get; private set; }
    public string Rua { get; private set; }
    public string Numero { get; private set; }
    public string Bairro { get; private set; }
    public string Cidade { get; private set; }

    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public bool Ativo { get; private set; }

    protected Endereco() { }

    public Endereco(
        int usuarioId,
        string rua,
        string numero,
        string bairro,
        string cidade,
        double latitude,
        double longitude)
    {
        if (string.IsNullOrWhiteSpace(rua))
            throw new Exception("Rua é obrigatória");

        if (latitude < -90 || latitude > 90)
            throw new Exception("Latitude inválida");

        if (longitude < -180 || longitude > 180)
            throw new Exception("Longitude inválida");

        UsuarioId = usuarioId;
        Rua = rua;
        Numero = numero;
        Bairro = bairro;
        Cidade = cidade;
        Latitude = latitude;
        Longitude = longitude;
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
    }
}
