
namespace JSDeposito.Core.Reports.DTOs
{
    public record ProdutoMaisVendidoDto(
    int ProdutoId,
    string Nome,
    int QuantidadeVendida,
    decimal TotalVendido
);
}
