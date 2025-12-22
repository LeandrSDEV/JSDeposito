using JSDeposito.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeposito.Core.Interfaces
{
    public interface IRefreshTokenRepository
    {
        void Salvar(RefreshToken token);
        RefreshToken Obter(string token);
        void Revogar(string token);
    }
}
