using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Security.Claims;
using Teamo.Core.Entities.Identity;

namespace TeamoWeb.API.Extensions
{
    public static class UserExtensions
    {
        // Get user by email with no extra profile info
        public static async Task<User> GetUserByEmail(this UserManager<User> userManager,
        ClaimsPrincipal user)
        {
            var userToReturn = await userManager.Users.FirstOrDefaultAsync(x =>
                x.Email == user.GetEmail());

            if (userToReturn == null) throw new AuthenticationException("User not found");

            return userToReturn;
        }

        // Get user by email with profile info
        public static async Task<User> GetUserByEmailWithProfile(this UserManager<User> userManager,
            ClaimsPrincipal user)
        {
            var userToReturn = await userManager.Users
                .Include(x => x.Major)
                .Include(x => x.Links)
                .Include(x => x.Skills)
                .FirstOrDefaultAsync(x => x.Email == user.GetEmail());

            if (userToReturn == null) throw new AuthenticationException("User not found");

            return userToReturn;
        }

        // Get email of the current user from ClaimsPrincipal
        public static string GetEmail(this ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Email)
                ?? throw new AuthenticationException("Email claim of the current user not found");

            return email;
        }
    }
}
