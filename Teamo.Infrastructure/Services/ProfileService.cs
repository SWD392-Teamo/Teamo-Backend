using Microsoft.AspNetCore.Identity;
using Teamo.Core.Entities;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Links;
using Teamo.Core.Specifications.StudentSkills;
using Teamo.Core.Specifications.Users;

namespace Teamo.Infrastructure.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public ProfileService(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<User> GetProfileAsync(int id)
        {
            var userSpec = new UserSpecification(id);
            return await _userService.GetUserWithSpec(userSpec);
        }

        public async Task<IdentityResult> UpdateProfileDescriptionAsync(User user)
        {
            return await _userService.UpdateUserAsync(user);
        }

        public async Task<bool> AddProfileSkillAsync(StudentSkill newSkill)
        {
            _unitOfWork.Repository<StudentSkill>().Add(newSkill);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> UpdateProfileSkillAsync(int userId, int skillId, StudentSkillLevel skillLevel)
        {
            var spec = new StudentSkillSpecification(skillId, userId);
            var profileSkill = await _unitOfWork.Repository<StudentSkill>().GetEntityWithSpec(spec);
            profileSkill.Level = skillLevel;
            _unitOfWork.Repository<StudentSkill>().Update(profileSkill);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> DeleteProfileSkillAsync(StudentSkill studentSkill)
        {
            _unitOfWork.Repository<StudentSkill>().Delete(studentSkill);
            return await _unitOfWork.Complete();
        }

        public async Task<StudentSkill> GetProfileSkillAsync(int studentSkillId)
        {
            return await _unitOfWork.Repository<StudentSkill>().GetByIdAsync(studentSkillId);
        }

        public async Task<bool> AddProfileLinkAsync(Link newLink)
        {
            _unitOfWork.Repository<Link>().Add(newLink);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> UpdateProfileLinkAsync(Link link)
        {
            _unitOfWork.Repository<Link>().Update(link);
            return await _unitOfWork.Complete();
        }

        public async Task<Link> GetLinkByIdAsync(int id)
        {
            var linkSpec = new LinkSpecification(id);
            var link = await _unitOfWork.Repository<Link>().GetEntityWithSpec(linkSpec);
            return link;
        }

        public async Task<bool> RemoveProfileLinkAsync(Link link)
        {
            _unitOfWork.Repository<Link>().Delete(link);
            return await _unitOfWork.Complete();
        }
    }
}