using JSDeposito.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace JSDeposito.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();
        public DbSet<Cupom> Cupons => Set<Cupom>();
    }
}
