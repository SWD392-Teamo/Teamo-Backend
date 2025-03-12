using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Specifications;

namespace Teamo.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<IReadOnlyList<User>> ListAllUsersAsync();
        Task<User> GetUserWithSpec(ISpecification<User> spec);
        Task<IReadOnlyList<User>> ListUsersAsync(ISpecification<User> spec);
        Task<int> CountAsync(ISpecification<User> spec);
        Task<string> GetUserRoleAsync(User user);
        Task<IdentityResult> AddUserToRoleAsync(User user, string role);
        Task<IdentityResult> CreateUserAsync(User user);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<User> GetUserByClaims(ClaimsPrincipal principal);
        Task<User> GetUserByIdAsync(int id);
        Task<IList<Claim>> GetUserClaims(User user);
    }
}
