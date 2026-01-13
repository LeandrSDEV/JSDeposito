using System.Security.Claims;

namespace JSDeposito.Api.UserExtensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUsuarioId(this ClaimsPrincipal user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (user.Identity?.IsAuthenticated != true)
            throw new UnauthorizedAccessException("Usuário não autenticado");

        var claim = user.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null)
            throw new UnauthorizedAccessException("Claim de usuário não encontrada");

        if (!int.TryParse(claim.Value, out var usuarioId))
            throw new UnauthorizedAccessException("Id do usuário inválido");

        return usuarioId;
    }
}
