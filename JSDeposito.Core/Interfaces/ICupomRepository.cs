using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface ICupomRepository
{
    Cupom ObterPorCodigo(string codigo);
    void Atualizar(Cupom cupom);
}
