using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeposito.Core.DTOs
{
    public class CriarCupomRequestDto
    {
        public string Codigo { get; set; } = string.Empty;
        public decimal? ValorDesconto { get; set; }
        public int? Percentual { get; set; }
        public DateTime ValidoAte { get; set; }
        public int LimiteUso { get; set; }
    }
}
