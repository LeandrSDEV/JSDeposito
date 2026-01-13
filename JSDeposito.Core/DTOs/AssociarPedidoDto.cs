using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeposito.Core.DTOs
{
    public class AssociarPedidoDto
    {
        public Guid TokenAnonimo { get; set; }
        public int UsuarioId { get; set; }
    }
}
