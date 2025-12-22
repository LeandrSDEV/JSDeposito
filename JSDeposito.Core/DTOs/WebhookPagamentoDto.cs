namespace JSDeposito.Core.DTOs;

public class WebhookPagamentoDto
{
    /// <summary>
    /// ID da transação no gateway (ex: Stripe, MercadoPago)
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// ID do pedido no seu sistema
    /// </summary>
    public int PedidoId { get; set; }

    /// <summary>
    /// Status retornado pelo gateway
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Valor efetivamente pago
    /// </summary>
    public decimal ValorPago { get; set; }

    /// <summary>
    /// Método de pagamento (PIX, CARTAO, BOLETO)
    /// </summary>
    public string Metodo { get; set; } = string.Empty;

    /// <summary>
    /// Data/hora do pagamento no gateway
    /// </summary>
    public DateTime PagoEm { get; set; }

    /// <summary>
    /// Assinatura para validação do webhook
    /// </summary>
    public string? Assinatura { get; set; }
}
