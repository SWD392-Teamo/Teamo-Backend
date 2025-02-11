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

        public async Task<User> GetProfileAsync(string userEmail)
        {
            var userSpec = new UserSpecification(userEmail);
            return await _userService.GetUserWithSpec(userSpec);
        }

        public async Task<IdentityResult> UpdateProfileDescriptionAsync(int userId, string description)
        {
            var userSpec = new UserSpecification(userId);
            var user = await _userService.GetUserWithSpec(userSpec);
            user.Description = description;
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

        public async Task<bool> AddProfileLinkAsync(Link newLink)
        {
            _unitOfWork.Repository<Link>().Add(newLink);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> UpdateProfileLinkAsync(int linkId, string linkName, string linkurl)
        {
            var linkSpec = new LinkSpecification(linkId);
            var profileLink = await _unitOfWork.Repository<Link>().GetEntityWithSpec(linkSpec);
            profileLink.Name = linkName;
            profileLink.Url = linkurl;
            _unitOfWork.Repository<Link>().Update(profileLink);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> RemoveProfileLinkAsync(int linkId)
        {
            var linkSpec = new LinkSpecification(linkId);
            var profileLink = await _unitOfWork.Repository<Link>().GetEntityWithSpec(linkSpec);
            _unitOfWork.Repository<Link>().Delete(profileLink);
            return await _unitOfWork.Complete();
        }
    }
}