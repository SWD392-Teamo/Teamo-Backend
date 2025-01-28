using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications;
using Teamo.Infrastructure.Data;

namespace Teamo.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly string _studentRoleName = "Student";

        public UserService(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> AddUserToRoleAsync(User user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<string> GetUserRoleAsync(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var userRole = userRoles.FirstOrDefault();
            return userRole;
        }

        private async Task<IList<User>> GetUsersInRoleAsync(string role)
        {
            return await _userManager.GetUsersInRoleAsync(role);
        }

        public async Task<User> GetUserWithSpec(ISpecification<User> spec)
        {
            return await ApplySpecification(_userManager.Users.AsQueryable(), spec).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<User>> ListAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<IReadOnlyList<User>> ListUsersAsync(ISpecification<User> spec)
        {
            var query = _userManager.Users.AsQueryable();
            var students = await GetUsersInRoleAsync(_studentRoleName);
            query = query.Where(u => students.Contains(u));
            return await ApplySpecification(query, spec).ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<User> spec)
        {
            var query = _userManager.Users.AsQueryable();
            var students = await GetUsersInRoleAsync(_studentRoleName);
            query = query.Where(u => students.Contains(u));
            return await ApplySpecification(query, spec).CountAsync();
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        private IQueryable<User> ApplySpecification(IQueryable<User> query, ISpecification<User> spec)
        {
            return UserSpecificationEvaluator.GetQuery(query, spec);
        }


    }
}
