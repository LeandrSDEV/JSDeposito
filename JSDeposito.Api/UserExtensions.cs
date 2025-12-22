using System.Security.Claims;

namespace JSDeposito.Api
{
    public static class UserExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                throw new UnauthorizedAccessException("Usuário não autenticado");

            return int.Parse(claim.Value);
        }
    }
}
