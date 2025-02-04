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

        //Display user in admin view
        public static UserDto? ToDto(this User? user)
        {
            if(user == null) return null;
            return new UserDto
            {
                Code = user.Code,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Gender = user.Gender.ToString(),
                Dob = user.Dob,
                ImgUrl = user.ImgUrl ?? null,
                Status = user.Status.ToString(),
                Description = user.Description ?? null,
                MajorCode = user.Major.Code,
                Links = (user.Links != null) ? user.Links.Select(l => l.ToDto()).ToList() : null,
                Skills = (user.Skills != null) ? user.Skills.Select(s => s.ToDto()).ToList() : null
            };
        }
    }
}
