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
        private readonly IUserService _userService;
        
        public ProfileController(IProfileService profileService, IUserService userService)
        {
            _profileService = profileService;
            _userService = userService;
        }

        //Display current user's profile
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            
            var userProfile = await _profileService.GetProfileAsync(user.Id);

            if(userProfile == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            return Ok(userProfile.ToProfileDto());
        }

        //Update description in user profile
        [HttpPatch("descriptions")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileDescription([FromBody] ProfileDto profile)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));

            user = profile.UpdateDescription(user);

            var result = await _profileService.UpdateProfileDescriptionAsync(user);

            if(result.Succeeded) return Ok(new ApiErrorResponse(200, "Profile description added successfully."));
            else return BadRequest(new ApiErrorResponse(400, "Failed to updated profile description."));
        }

        //Add a new skill to user profile
        [HttpPost("skills")]
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
        [HttpPatch("skills/{studentSkillId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileSkill(int studentSkillId, [FromBody] StudentSkillToUpsertDto studentSkillDto)
        {
            var studentSkill = await _profileService.GetProfileSkillAsync(studentSkillId);
            if(studentSkill == null) return NotFound(new ApiErrorResponse(404, "Skill not found in profile."));

            studentSkill = studentSkillDto.ToEntity(studentSkill);

            var result = await _profileService.UpdateProfileSkillAsync(studentSkill);

            if(result) return Ok(new ApiErrorResponse(200, "Skill updated successfully."));
            else return BadRequest(new ApiErrorResponse(400, "Failed to update skill."));
        }

        //Delete a skill in user profile
        [HttpDelete("skills/{studentSkillId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> DeleteProfileSkill(int studentSkillId)
        {
            var studentSkill = await _profileService.GetProfileSkillAsync(studentSkillId);
            if(studentSkill == null) return NotFound(new ApiErrorResponse(404, "Skill not found in profile."));

            var result = await _profileService.DeleteProfileSkillAsync(studentSkill);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to delete skill from profile."));
            return Ok(new ApiErrorResponse(200, "Skill deleted from profile successfully"));   
        }

        //Add a new link to user profile
        [HttpPost("links")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> AddProfileLink([FromBody] LinkToUpsertDto linkDto)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            linkDto.StudentId = user.Id;

            if(linkDto == null || string.IsNullOrEmpty(linkDto.Name) || string.IsNullOrEmpty(linkDto.Url)) 
                return BadRequest(new ApiErrorResponse(400, "Please input all required fields."));

            var link = linkDto.ToEntity();

            var result = await _profileService.AddProfileLinkAsync(link);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to add link to profile."));
            return Ok(new ApiErrorResponse(200, "Link added to profile successfully."));
        }

        //Update a link in user profile
        [HttpPatch("links/{linkId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileLink(int linkId, [FromBody] LinkToUpsertDto linkDto)
        {            
            var link = await _profileService.GetLinkByIdAsync(linkId);
            if(link == null) return NotFound(new ApiErrorResponse(404, "Link not found"));

            link = linkDto.ToEntity(link);
            var result = await _profileService.UpdateProfileLinkAsync(link);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to update link in profile."));
            return Ok(new ApiErrorResponse(200, "Link updated in profile successfully."));
        }

        //Remove a link from user profile
        [HttpDelete("links/{linkId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> RemoveProfileLink(int linkId)
        {
            var link = await _profileService.GetLinkByIdAsync(linkId);
            if(link == null) return NotFound(new ApiErrorResponse(404, "Link not found"));
            
            var result = await _profileService.RemoveProfileLinkAsync(link);
            
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to remove link from profile"));
            return Ok(new ApiErrorResponse(200, "Link removed from profile successfully."));
        }
    }
}