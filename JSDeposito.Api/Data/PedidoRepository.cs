using JSDeposito.Api.Data;
using JSDeposito.Core.Entities;
using JSDeposito.Core.Enums;
using JSDeposito.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JSDeposito.Api.Data;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Criar(Pedido pedido)
    {
        _context.Pedidos.Add(pedido);
        _context.SaveChanges();
    }

    public Pedido? ObterPorId(int id)
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefault(p => p.Id == id);
    }


    public void Atualizar(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
        _context.SaveChanges();
    }
    public Pedido? ObterPorTokenAnonimo(Guid token)
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefault(p => p.TokenAnonimo == token);
    }

    public void Remover(Pedido pedido)
    {
        _context.Pedidos.Remove(pedido);
        _context.SaveChanges();
    }

    public Pedido? ObterPedidoAbertoDoUsuario(int usuarioId)
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefault(p =>
                p.UsuarioId == usuarioId &&
                p.Status == PedidoStatus.Criado
            );
    }
}
