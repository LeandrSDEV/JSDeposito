using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface IPromocaoFreteRepository
{
    PromocaoFrete ObterPorId(int id);
    IEnumerable<PromocaoFrete> Listar();
    void Criar(PromocaoFrete promocao);
    void Atualizar(PromocaoFrete promocao);
    PromocaoFrete? ObterPromocaoAtiva();
}
