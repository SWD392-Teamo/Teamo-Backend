using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces.Services;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
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

            if(userProfile == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));

            return Ok(userProfile.ToProfileDto());
        }

        //Update description in user profile
        [HttpPatch("des-update")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileDescription([FromBody] ProfileUpdateDto profile)
        {
            var result = await _profileService.UpdateProfileDescriptionAsync(profile.UserId, profile.Description);

            if(result.Succeeded) return Ok(new ApiErrorResponse(200, "Profile description added successfully."));
            else return BadRequest(new ApiErrorResponse(400, "Failed to updated profile description."));
        }

        //Add a new skill to user profile
        [HttpPost("skill-add")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> AddProfileSkill([FromBody] ProfileUpdateDto profile)
        {
            var newSkill = profile.StudentSkill;
            if(newSkill == null) return BadRequest(new ApiErrorResponse(400, "No skill selected."));

            Enum.TryParse(newSkill.SkillLevel, out StudentSkillLevel level);
            var studentSkill = new StudentSkill()
                {
                    SkillId = newSkill.SkillId, 
                    StudentId = profile.UserId,
                    Level = level
                };

            var result = await _profileService.AddProfileSkillAsync(studentSkill);

            if(result) return Ok(new ApiErrorResponse(200, "Skill added to profile successfully."));
            else return BadRequest(new ApiErrorResponse(400, "Failed to add skill to profile."));
        }

        //Update a skill level in user profile
        [HttpPatch("skill-update")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileSkill([FromBody] ProfileUpdateDto profile)
        {
            var updateSkill = profile.StudentSkill;
            if(updateSkill == null) return BadRequest(new ApiErrorResponse(400, "No skill selected."));

            Enum.TryParse(updateSkill.SkillLevel, out StudentSkillLevel updateLevel);

            var result = await _profileService.UpdateProfileSkillAsync(profile.UserId, updateSkill.SkillId, updateLevel);

            if(result) return Ok(new ApiErrorResponse(200, "Skill updated successfully."));
            else return BadRequest(new ApiErrorResponse(400, "Failed to update skill."));
        }

        //Add a new link to user profile
        [HttpPost("link-add")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> AddProfileLink([FromBody] ProfileUpdateDto profile)
        {
            var newLink = profile.Link;
            if(newLink == null) return BadRequest(new ApiErrorResponse(400, "Please input all required fields for link."));

            var link = new Link()
                {
                    Name = newLink.Name,
                    Url = newLink.Url,
                    StudentId = profile.UserId
                };

            var result = await _profileService.AddProfileLinkAsync(link);

            if(result) return Ok(new ApiErrorResponse(200, "Link added to profile successfully."));
            else return BadRequest(new ApiErrorResponse(400, "Failed to add link to profile."));
        }

        //Update a link in user profile
        [HttpPatch("link-update")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileLink([FromBody] ProfileUpdateDto profile)
        {
            var updateLink = profile.Link;
            if(updateLink == null) return BadRequest(new ApiErrorResponse(400, "No link in profile selected."));

            var result = await _profileService.UpdateProfileLinkAsync(updateLink.Id, updateLink.Name, updateLink.Url);

            if(result) return Ok(new ApiErrorResponse(200, "Link updated in profile successfully."));
            else return BadRequest(new ApiErrorResponse(400, "Failed to update link in profile."));
        }

        //Remove a link from user profile
        [HttpDelete("link-del")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> RemoveProfileLink([FromBody] ProfileUpdateDto profile)
        {
            var delLink = profile.Link;
            if(delLink == null) return BadRequest(new ApiErrorResponse(400, "No link in profile selected."));

            var result = await _profileService.RemoveProfileLinkAsync(delLink.Id);
            
            if(result) return Ok(new ApiErrorResponse(200, "Link removed from profile successfully."));
            else return BadRequest(new ApiErrorResponse(400, "Failed to remove link from profile"));
        }
    }
}