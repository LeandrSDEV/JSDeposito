using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSDeposito.Core.Exceptions
{
    public class EstoqueInsuficienteException : Exception
    {
        public int EstoqueAtual { get; }

        public EstoqueInsuficienteException(int estoqueAtual)
            : base($"Estoque insuficiente. Atual: {estoqueAtual}")
        {
            EstoqueAtual = estoqueAtual;
        }
    }
}
