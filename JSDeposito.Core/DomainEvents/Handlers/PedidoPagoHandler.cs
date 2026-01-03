using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeposito.Core.DomainEvents.Handlers
{
    public class PedidoPagoHandler
    {
        private readonly ILogger<PedidoPagoHandler> _logger;

        public PedidoPagoHandler(ILogger<PedidoPagoHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(PedidoPagoEvent evt)
        {
            _logger.LogInformation("Pedido pago | PedidoId: {PedidoId}", evt.PedidoId);
            return Task.CompletedTask;
        }
    }

}
