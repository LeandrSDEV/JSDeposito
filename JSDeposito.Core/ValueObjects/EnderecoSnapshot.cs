namespace JSDeposito.Core.ValueObjects;


public record EnderecoSnapshot(
    string Rua,
    string Numero,
    string Bairro,
    string Cidade,
    double Latitude,
    double Longitude
);
