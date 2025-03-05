using System.Security.Authentication;
using System.Security.Claims;
using Teamo.Core.Entities.Identity;
using TeamoWeb.API.Dtos;

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

        // Get id of the current user from ClaimsPrincipal
        public static int GetId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new AuthenticationException("Name identifier claim of the current user not found");

            return int.Parse(id);
        }

        //Display user info
        public static UserDto? ToDto(this User? user)
        {
            if(user == null) return null;
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty
            };
        }
    }
}
