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
        public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
        public DbSet<Endereco> Enderecos => Set<Endereco>();
        public DbSet<PromocaoFrete> PromocaoFretes => Set<PromocaoFrete>();
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>(entity =>
            {
                entity.Property(p => p.Preco)
                      .HasColumnType("decimal(10,2)");

            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.Property(p => p.Total)
                      .HasColumnType("decimal(10,2)");
                entity.Property(p => p.Desconto)
                      .HasColumnType("decimal(10,2)");
                entity.Property(p => p.ValorFrete)
                      .HasColumnType("decimal(10,2)");
                entity.OwnsOne(p => p.EnderecoEntrega, endereco =>
                {
                    endereco.Property(e => e.Rua).HasMaxLength(200);
                    endereco.Property(e => e.Cidade).HasMaxLength(100);
                });
            });

            modelBuilder.Entity<Pagamento>(entity =>
            {
                entity.Property(p => p.Valor)
                .HasColumnType("decimal(10,2)");
            });

            modelBuilder.Entity<ItemPedido>(entity =>
            {
                entity.Property(i => i.PrecoUnitario)
                .HasColumnType("decimal(10,2)");         
            });

            modelBuilder.Entity<Cupom>(entity =>
            {
                entity.Property(c => c.ValorDesconto)
                .HasColumnType("decimal(10,2)");
            });
        }
}
}
