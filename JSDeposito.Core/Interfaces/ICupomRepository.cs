using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface ICupomRepository
{
    void Criar(Cupom cupom);
    Cupom ObterPorCodigo(string codigo);
    void Atualizar(Cupom cupom);
    
}
