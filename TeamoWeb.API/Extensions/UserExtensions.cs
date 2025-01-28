using System.Security.Authentication;
using System.Security.Claims;

namespace TeamoWeb.API.Extensions
{
    public static class UserExtensions
    {
        // Get email of the current user from ClaimsPrincipal
        public static string GetEmail(this ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Email)
                ?? throw new AuthenticationException("Email claim of the current user not found");

            return email;
        }

        // Get role of the current user from ClaimsPrincipal
        public static string GetRole(this ClaimsPrincipal user)
        {
            var role = user.FindFirstValue(ClaimTypes.Role)
                ?? throw new AuthenticationException("Role claim of the current user not found");

            return role;
        }
    }
}
