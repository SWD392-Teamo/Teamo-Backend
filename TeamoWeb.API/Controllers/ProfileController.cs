using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces.Services;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Extensions;

namespace TeamoWeb.API.Controllers
{
    public class ProfileController : BaseApiController
    {
        private readonly IProfileService _profileService;
        
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var userProfile = await _profileService.GetProfileAsync(User.GetEmail());
            return Ok(userProfile.ToDto());
        }

        //Update description in user profile
        [HttpPatch("des-update")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileDescription(int userId, string description)
        {
            var result = await _profileService.UpdateProfileDescriptionAsync(userId, description);
            if(result.Succeeded) return Ok();
            else return BadRequest();
        }

        //Add a new skill to user profile
        [HttpPost("skill-add")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> AddProfileSkill(int userId, StudentSkillDto newSkill)
        {
            Enum.TryParse(newSkill.SkillLevel, out StudentSkillLevel level);
            var studentSkill = new StudentSkill()
                {
                    SkillId = newSkill.SkillId, 
                    StudentId = userId,
                    Level = level
                };
            var result = await _profileService.AddProfileSkillAsync(studentSkill);
            if(result) return Ok();
            else return BadRequest();
        }

        //Update a skill level in user profile
        [HttpPatch("skill-update")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileSkill(int userId, StudentSkillDto updateSkill)
        {
            Enum.TryParse(updateSkill.SkillLevel, out StudentSkillLevel updateLevel);
            var result = await _profileService.UpdateProfileSkillAsync(userId, updateSkill.SkillId, updateLevel);
            if(result) return Ok();
            else return BadRequest();
        }

        //Add a new link to user profile
        [HttpPost("link-add")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> AddProfileLink(int userId, LinkDto newLink)
        {
            var link = new Link()
                {
                    Name = newLink.Name,
                    Url = newLink.Url,
                    StudentId = userId
                };
            var result = await _profileService.AddProfileLinkAsync(link);
            if(result) return Ok();
            else return BadRequest();
        }

        //Update a link in user profile
        [HttpPatch("link-update")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileLink(LinkDto link)
        {
            var result = await _profileService.UpdateProfileLinkAsync(link.Id, link.Name, link.Url);
            if(result) return Ok();
            else return BadRequest();
        }

        //Remove a link from user profile
        [HttpDelete("link-del")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> RemoveProfileLink(int linkId)
        {
            var result = await _profileService.RemoveProfileLinkAsync(linkId);
            if(result) return Ok();
            else return BadRequest();
        }
    }
}