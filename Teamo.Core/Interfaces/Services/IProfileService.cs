using Microsoft.AspNetCore.Identity;
using Teamo.Core.Entities;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;

namespace Teamo.Core.Interfaces.Services
{
    public interface IProfileService
    {
        Task<User> GetProfileAsync(string userEmail);
        Task<IdentityResult> UpdateProfileDescriptionAsync(int userId, string description);
        Task<bool> AddProfileSkillAsync(StudentSkill newSkill);
        Task<bool> UpdateProfileSkillAsync(int userId, int skillId, StudentSkillLevel skillLevel);
        Task<bool> AddProfileLinkAsync(Link newLink);
        Task<bool> UpdateProfileLinkAsync(int linkId, string linkName, string linkUrl);
        Task<bool> RemoveProfileLinkAsync(int linkId);
    }
}