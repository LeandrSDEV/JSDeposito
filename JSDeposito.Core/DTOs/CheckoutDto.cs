using JSDeposito.Core.Enums;

namespace JSDeposito.Core.DTOs;

public class CheckoutDto
{
    public int PedidoId { get; set; }
    public int ClienteId { get; set; }
    public int EnderecoId { get; set; }
    public string? CodigoCupom { get; set; }
    public TipoPagamento TipoPagamento { get; set; }
}
