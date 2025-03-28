using Microsoft.AspNetCore.Identity;
using Teamo.Core.Entities;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;

namespace Teamo.Core.Interfaces.Services
{
    public interface IProfileService
    {
        Task<User> GetProfileAsync(int id);
        Task<IdentityResult> UpdateProfileDescriptionAsync(User user);
        Task<bool> AddProfileSkillsAsync(List<StudentSkill> newSkills);
        Task<StudentSkill> UpdateProfileSkillAsync(StudentSkill studentSkill);
        Task<bool> DeleteProfileSkillAsync(StudentSkill studentSkill);
        Task<StudentSkill> GetProfileSkillAsync(int studentSkillId);
        Task<bool> AddProfileLinksAsync(List<Link> newLinks);
        Task<bool> UpdateProfileLinkAsync(Link link);
        Task<Link> GetLinkByIdAsync(int id);
        Task<bool> RemoveProfileLinkAsync(Link link);
    }
}