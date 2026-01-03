using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeposito.Core.DomainEvents
{
    public abstract record DomainEvent(DateTime OccurredOn);

    public record PedidoPagoEvent(int PedidoId)
    : DomainEvent(DateTime.UtcNow);
}
