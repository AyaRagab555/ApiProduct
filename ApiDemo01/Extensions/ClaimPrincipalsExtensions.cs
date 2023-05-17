using System.Security.Claims;

namespace ApiDemo01.Extensions
{
    public static class ClaimPrincipalsExtensions
    {
        public static string RetrieveEmailFromPrincipal(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.Email);
    }
}
