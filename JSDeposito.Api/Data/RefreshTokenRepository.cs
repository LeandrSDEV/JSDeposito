using JSDeposito.Core.Entities;
using JSDeposito.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JSDeposito.Api.Data;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Salvar(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        _context.SaveChanges();
    }

    public RefreshToken? Obter(string token)
    {
        return _context.RefreshTokens
            .FirstOrDefault(t => t.Token == token);
    }

    public void Revogar(string token)
    {
        var refresh = Obter(token);

        if (refresh == null) return;

        refresh.Revogar();
        _context.SaveChanges();
    }
}
