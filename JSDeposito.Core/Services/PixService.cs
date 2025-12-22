using JSDeposito.Core.DTOs;
using System.Text;

namespace JSDeposito.Core.Services;

public class PixService
{
    public PixDto GerarPix(decimal valor, string descricao)
    {
        // ⚠️ MOCK — depois você troca por integração real (Banco, Mercado Pago, Gerencianet, etc)

        var txId = Guid.NewGuid().ToString("N")[..25];

        var copiaECola = $"00020126360014BR.GOV.BCB.PIX0114+55999999999952040000530398654{valor:F2}5802BR5920JS DEPOSITO6009ARACAJU62070503***6304ABCD";

        var qrCodeBase64 = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(copiaECola)
        );

        return new PixDto
        {
            TxId = txId,
            CopiaECola = copiaECola,
            QrCodeBase64 = qrCodeBase64,
            ExpiraEm = DateTime.UtcNow.AddMinutes(30)
        };
    }
}