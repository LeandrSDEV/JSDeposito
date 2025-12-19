using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Api.Data;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public Cliente ObterPorId(int id)
        => _context.Clientes.FirstOrDefault(c => c.Id == id);

    public Cliente ObterPorEmail(string email)
        => _context.Clientes.FirstOrDefault(c => c.Email == email);

    public void Criar(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        _context.SaveChanges();
    }
}
