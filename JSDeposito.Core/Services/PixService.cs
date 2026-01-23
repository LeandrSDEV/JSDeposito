using Microsoft.Extensions.Logging;
using JSDeposito.Core.DTOs;
using System.Text;
using QRCoder;

namespace JSDeposito.Core.Services;

public class PixService
{
    private readonly ILogger<PixService> _logger;

    public PixService(ILogger<PixService> logger)
    {
        _logger = logger;
    }

    public PixDto GerarPix(decimal valor, string descricao)
    {
        _logger.LogInformation(
            "Gerando PIX | Valor: {Valor} | Descrição: {Descricao}",
            valor,
            descricao);

        if (valor <= 0)
            throw new ArgumentException("Valor do Pix deve ser maior que zero");

        // ⚠️ MOCK — depois troca por integração real
        var txId = Guid.NewGuid().ToString("N")[..25];

        var copiaECola =
            $"00020126360014BR.GOV.BCB.PIX0114+55999999999952040000530398654{valor:F2}5802BR5920JS DEPOSITO6009ARACAJU62070503***6304ABCD";

        var qrPngBytes = new PngByteQRCode(new QRCodeGenerator().CreateQrCode(copiaECola, QRCodeGenerator.ECCLevel.Q)).GetGraphic(10);

        var qrCodeBase64 = Convert.ToBase64String(qrPngBytes);

        _logger.LogInformation(
            "PIX gerado com sucesso | TxId: {TxId} | Expira em: {ExpiraEm}",
            txId,
            DateTime.UtcNow.AddMinutes(30));

        return new PixDto
        {
            TxId = txId,
            CopiaECola = copiaECola,
            QrCodeBase64 = qrCodeBase64,
            ExpiraEm = DateTime.UtcNow.AddMinutes(30)
        };
    }
}