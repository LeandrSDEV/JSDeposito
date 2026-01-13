using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeposito.Core.DTOs
{
    public record ConflitoCarrinhoResponse(
    bool ConflitoCarrinho,
    int PedidoUsuarioId,
    int PedidoAnonimoId
);
}
