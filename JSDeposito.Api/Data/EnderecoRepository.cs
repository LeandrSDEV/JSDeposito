using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;

namespace JSDeposito.Api.Data;

public class EnderecoRepository : IEnderecoRepository
{
    private readonly AppDbContext _context;

    public EnderecoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Endereco ObterPorId(int id)
        => _context.Enderecos.FirstOrDefault(e => e.Id == id);

    public List<Endereco> ObterPorCliente(int clienteId)
        => _context.Enderecos
            .Where(e => e.ClienteId == clienteId)
            .ToList();

    public void Criar(Endereco endereco)
    {
        _context.Enderecos.Add(endereco);
        _context.SaveChanges();
    }
}
