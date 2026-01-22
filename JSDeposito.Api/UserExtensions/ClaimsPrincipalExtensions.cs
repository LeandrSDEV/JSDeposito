using System.Security.Claims;

namespace JSDeposito.Api.UserExtensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(idClaim) || !int.TryParse(idClaim, out var userId))
            throw new InvalidOperationException("Usuário não autenticado ou claim de identificação inválido.");

        return userId;
    }
}
