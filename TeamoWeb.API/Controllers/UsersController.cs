using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Users;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{
    //Managing users (students)
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IProfileService _profileService;

        public UsersController(IUserService userService, IProfileService profileService)
        {
            _userService = userService;
            _profileService = profileService;
        }

        //Get all users with spec
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<ProfileDto>>> GetUsers([FromQuery] UserSpecParams userSpecParams)
        {
            var userSpec = new UserSpecification(userSpecParams);
            var users = await _userService.ListUsersAsync(userSpec);
            var count = await _userService.CountAsync(userSpec);
            var usersToProfileDto = users.Select(u => u.ToProfileDto()).ToList();
            var pagination = new Pagination<ProfileDto>(userSpecParams.PageIndex,userSpecParams.PageSize,count,usersToProfileDto);
            return Ok(pagination);
        }

        //Get user by id
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult<ProfileDto?>> GetUserById (int id)
        {
            var userSpec = new UserSpecification(id);
            var user = await _userService.GetUserWithSpec(userSpec);

            if (user == null) return NotFound(new ApiErrorResponse(404, "User not found."));
            return Ok(user.ToProfileDto());
        }

        //Ban user
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProfileDto>> BanUser (int id)
        {
            var userSpec = new UserSpecification(id);
            var user = await _userService.GetUserWithSpec(userSpec);
            if(user == null) return NotFound(new ApiErrorResponse(404, "User not found."));

            if(user.Status != UserStatus.Active) return BadRequest(new ApiErrorResponse(400, "Failed to ban user."));
            user.Status = UserStatus.Inactive;
            
            var result = await _userService.UpdateUserAsync(user);
            
            if (result.Succeeded) return Ok(new ApiErrorResponse(200, "Banned user successfully."));
            else return BadRequest();
        }

        //Display current user's profile
        [HttpGet("{userId}/profile")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<UserDto>> GetProfile(int userId)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            
            var userProfile = await _profileService.GetProfileAsync(userId);

            if(userProfile == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            return Ok(userProfile.ToProfileDto());
        }

        //Update description in user profile
        [HttpPatch("{userId}/profile/descriptions")]
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
        [HttpPost("{userId}/profile/skills")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> AddProfileSkill([FromBody] StudentSkillToUpsertDto studentSkillDto)
        {
             var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            studentSkillDto.StudentId = user.Id;

            var studentSkill = studentSkillDto.ToEntity();
            var result = await _profileService.AddProfileSkillAsync(studentSkill);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to add skill to profile."));
            return Ok(new ApiErrorResponse(200, "Skill added to profile successfully."));
        }

        //Update a skill level in user profile
        [HttpPatch("{userId}/profile/skills/{studentSkillId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileSkill(int studentSkillId, [FromBody] StudentSkillToUpsertDto studentSkillDto)
        {
            var studentSkill = await _profileService.GetProfileSkillAsync(studentSkillId);
            if(studentSkill == null) return NotFound(new ApiErrorResponse(404, "Skill not found in profile."));

            studentSkill = studentSkillDto.ToEntity(studentSkill);

            var result = await _profileService.UpdateProfileSkillAsync(studentSkill);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to update skill."));
            return Ok(new ApiErrorResponse(200, "Skill updated successfully."));
        }

        //Delete a skill in user profile
        [HttpDelete("{userId}/profile/skills/{studentSkillId}")]
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
        [HttpPost("{userId}/profile/links")]
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
        [HttpPatch("{userId}/profile/links/{linkId}")]
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
        [HttpDelete("{userId}/profile/links/{linkId}")]
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