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
    }
}
