using System.Security.Claims;

namespace JSDeposito.Api.UserExtensions
{
    public static class UserExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                throw new Exception("Claim NameIdentifier não encontrada no token");

            if (!int.TryParse(claim.Value, out var userId))
                throw new Exception("Claim NameIdentifier não é um inteiro válido");

            return userId;
        }
    }
}