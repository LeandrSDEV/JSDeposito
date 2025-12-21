using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeposito.Core.Interfaces
{
    public interface IGeocodingService
    {
        Task<(double latitude, double longitude)> ObterCoordenadasAsync(string enderecoCompleto);
    }

}
