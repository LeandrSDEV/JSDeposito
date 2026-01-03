using JSDeposito.Core.Entities;
using JSDeposito.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeposito.Tests.Helpers.Builders
{
    public class PedidoBuilder
    {
        private readonly Pedido _pedido = new();

        public PedidoBuilder ComStatus(PedidoStatus status)
        {
            _pedido.DefinirStatus(status);
            return this;
        }

        public Pedido Build() => _pedido;
    }
}
