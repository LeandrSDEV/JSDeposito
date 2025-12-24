using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeposito.Core.DTOs
{
    public record RegisterRequest(
    string Nome,
    string Email,
    string Telefone,
    string Senha
);
}
