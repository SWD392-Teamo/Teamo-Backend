using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IUploadService _uploadService;
        private readonly IConfiguration _config;

        public UsersController(
            IUserService userService, 
            IProfileService profileService, 
            IUploadService uploadService,
            IConfiguration config)
        {
            _userService = userService;
            _profileService = profileService;
            _uploadService = uploadService;
            _config = config;
        }

        //Get all users with spec
        [Cache(1000)]
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
        [Cache(1000)]
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
        [InvalidateCache("/users")]
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
            
            if (result.Succeeded) return Ok(user.ToProfileDto());
            else return BadRequest(new ApiErrorResponse(400, "Failed to ban user."));
        }

        //Display current user's profile
        [Cache(1000)]
        [HttpGet("{userId}/profile")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<UserDto>> GetProfile(int userId)
        {
            if (User.GetId() != userId) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            
            var userProfile = await _profileService.GetProfileAsync(userId);

            if(userProfile == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            return Ok(userProfile.ToProfileDto());
        }

        //Update description in user profile
        [InvalidateCache("/profile")]
        [HttpPatch("{userId}/profile/descriptions")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ProfileDto>> UpdateProfileDescription([FromBody] ProfileDto profile)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));

            user = profile.UpdateDescription(user);

            var result = await _profileService.UpdateProfileDescriptionAsync(user);

            if(result.Succeeded) return Ok(user.ToProfileDto());
            else return BadRequest(new ApiErrorResponse(400, "Failed to updated profile description."));
        }

        //Add a new skill to user profile
        [InvalidateCache("/profile")]
        [HttpPost("{userId}/profile/skills")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<StudentSkillDto>> AddProfileSkill([FromBody] StudentSkillToUpsertDto studentSkillDto)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            studentSkillDto.StudentId = user.Id;

            var studentSkill = studentSkillDto.ToEntity();
            studentSkill = await _profileService.AddProfileSkillAsync(studentSkill);

            if(studentSkill == null) return BadRequest(new ApiErrorResponse(400, "Failed to add skill to profile."));
            return Ok(studentSkill.ToDto());
        }

        //Update a skill level in user profile
        [InvalidateCache("/profile")]
        [HttpPatch("{userId}/profile/skills/{studentSkillId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<StudentSkillDto>> UpdateProfileSkill(int studentSkillId, [FromBody] StudentSkillToUpsertDto studentSkillDto)
        {
            var studentSkill = await _profileService.GetProfileSkillAsync(studentSkillId);
            if(studentSkill == null) return NotFound(new ApiErrorResponse(404, "Skill not found in profile."));

            studentSkill = studentSkillDto.ToEntity(studentSkill);

            var result = await _profileService.UpdateProfileSkillAsync(studentSkill);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to update skill."));
            return Ok(studentSkill.ToDto());
        }

        //Delete a skill in user profile
        [InvalidateCache("/profile")]
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
        [InvalidateCache("/profile")]
        [HttpPost("{userId}/profile/links")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<LinkDto>> AddProfileLink([FromBody] LinkToUpsertDto linkDto)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            linkDto.StudentId = user.Id;

            if(linkDto == null || string.IsNullOrEmpty(linkDto.Name) || string.IsNullOrEmpty(linkDto.Url)) 
                return BadRequest(new ApiErrorResponse(400, "Please input all required fields."));

            var link = linkDto.ToEntity();

            link = await _profileService.AddProfileLinkAsync(link);

            if(link == null) return BadRequest(new ApiErrorResponse(400, "Failed to add link to profile."));
            return Ok(link.ToDto());
        }

        //Update a link in user profile
        [InvalidateCache("/profile")]
        [HttpPatch("{userId}/profile/links/{linkId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<LinkDto>> UpdateProfileLink(int linkId, [FromBody] LinkToUpsertDto linkDto)
        {            
            var link = await _profileService.GetLinkByIdAsync(linkId);
            if(link == null) return NotFound(new ApiErrorResponse(404, "Link not found"));

            link = linkDto.ToEntity(link);
            var result = await _profileService.UpdateProfileLinkAsync(link);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to update link in profile."));
            return Ok(link.ToDto());
        }

        //Remove a link from user profile
        [InvalidateCache("/profile")]
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
        // Update profile image
        [InvalidateCache("/profile")]
        [HttpPost("{userId}/profile/image")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UploadProfileImage(int userId, IFormFile image) 
        {
            var currentUser = await _userService.GetUserByIdAsync(userId);
            if(currentUser == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            if(image == null) return BadRequest(new ApiErrorResponse(400, "No image found"));

            var imgUrl = await _uploadService.UploadFileAsync(
                image.OpenReadStream(), 
                image.FileName, 
                image.ContentType,
                _config["Firebase:ProfileImagesUrl"]);

            currentUser.ImgUrl = imgUrl;

            var result = await _userService.UpdateUserAsync(currentUser);
            
            if (!result.Succeeded) return BadRequest(new ApiErrorResponse(400, "Failed to upload image."));

            return Ok(new ApiErrorResponse(200, "Image uploaded successfully."));
        }
    }
}