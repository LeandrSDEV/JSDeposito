using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeposito.Core.DTOs
{
    public class PixDto
    {
        public string TxId { get; set; } = string.Empty;
        public string CopiaECola { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
        public DateTime ExpiraEm { get; set; }
    }
}
