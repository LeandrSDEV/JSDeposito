using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JSDeposito.Api.Data;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public Usuario? ObterPorEmail(string email)
    {
        return _context.Usuarios
            .AsNoTracking()
            .FirstOrDefault(u => u.Email == email);
    }

    public Usuario? ObterPorId(int id)
    {
        return _context.Usuarios
            .AsNoTracking()
            .FirstOrDefault(u => u.Id == id);
    }
    public void Adicionar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
    }
}
