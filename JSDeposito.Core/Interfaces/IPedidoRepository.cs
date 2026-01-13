using JSDeposito.Core.Entities;

namespace JSDeposito.Core.Interfaces;

public interface IPedidoRepository
{
    void Criar(Pedido pedido);
    Pedido? ObterPorId(int id);
    void Atualizar(Pedido pedido);
    Pedido? ObterPorTokenAnonimo(Guid token);
    void Remover(Pedido pedido);
    Pedido? ObterPedidoAbertoDoUsuario(int usuarioId);

}
