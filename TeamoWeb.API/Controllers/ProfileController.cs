using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Groups;
using Teamo.Infrastructure.Services;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{
    public class ProfileController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IProfileService _profileService;
        private readonly IUploadService _uploadService;
        private readonly IGroupService _groupService;
        private readonly IConfiguration _config;

        public ProfileController
        (
            IUserService userService,
            IProfileService profileService, 
            IUploadService uploadService,
            IConfiguration config,
            IGroupService groupService
        )
        {
            _userService = userService;
            _profileService = profileService;
            _uploadService = uploadService;
            _config = config;
            _groupService = groupService;
        }

        //Display current user's profile
        [Cache(1000)]
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (User == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            
            var userProfile = await _profileService.GetProfileAsync(user.Id);

            if(userProfile == null) return BadRequest(new ApiErrorResponse(401, "Unable to retrieve profile."));
            return Ok(userProfile.ToProfileDto());
        }

        //Update description in user profile
        [InvalidateCache("/profile")]
        [HttpPatch("description")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ProfileDto>> UpdateProfileDescription([FromBody] ProfileDto profile)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));

            user = profile.UpdateDescription(user);

            var result = await _profileService.UpdateProfileDescriptionAsync(user);

            if(result.Succeeded) return Ok(user.ToProfileDto());
            else return BadRequest(new ApiErrorResponse(400, "Failed to update profile description."));
        }

        //Add new skills to user profile
        [InvalidateCache("/profile")]
        [HttpPost("skills")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> AddProfileSkills([FromBody] StudentSkillToUpsertDto[] newStudentSkillsDto)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));

            foreach (var s in newStudentSkillsDto)
            {
                s.StudentId = user.Id;
            }

            var newStudentSkills = newStudentSkillsDto.Select(s => s.ToEntity()).ToList();
            var result = await _profileService.AddProfileSkillsAsync(newStudentSkills);

            if(result) {
                var updatedProfile = await _profileService.GetProfileAsync(user.Id);
                return Ok(updatedProfile.ToProfileDto());
            }
            else return BadRequest(new ApiErrorResponse(400, "Failed to add skills to profile."));
        }

        //Update a skill level in user profile
        [InvalidateCache("/profile")]
        [HttpPatch("skills/{studentSkillId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<StudentSkillDto>> UpdateProfileSkill(int studentSkillId, [FromBody] StudentSkillToUpsertDto studentSkillDto)
        {
            var studentSkill = await _profileService.GetProfileSkillAsync(studentSkillId);
            if(studentSkill == null) return NotFound(new ApiErrorResponse(404, "Skill not found in profile."));

            studentSkill = studentSkillDto.ToEntity(studentSkill);

            studentSkill = await _profileService.UpdateProfileSkillAsync(studentSkill);
            return Ok(studentSkill.ToDto());
        }

        //Delete a skill in user profile
        [InvalidateCache("/profile")]
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
        [InvalidateCache("/profile")]
        [HttpPost("links")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> AddProfileLinks([FromBody] LinkToUpsertDto[] newLinksDto)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null) return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));

            foreach (var l in newLinksDto)
            {
                l.StudentId = user.Id;
            }

            var newLinks = newLinksDto.Select(l => l.ToEntity()).ToList();

            var result = await _profileService.AddProfileLinksAsync(newLinks);

            if(result) 
            {
                var updatedProfile = await _profileService.GetProfileAsync(user.Id);
                return Ok(updatedProfile.ToProfileDto());
            }
            else return BadRequest(new ApiErrorResponse(400, "Failed to add link to profile."));
        }

        //Update a link in user profile
        [InvalidateCache("/profile")]
        [HttpPatch("links/{linkId}")]
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

        // Update profile image
        [InvalidateCache("/profile")]
        [HttpPost("image")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UploadProfileImage(IFormFile image) 
        {
            var currentUser = await _userService.GetUserByClaims(HttpContext.User);
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

        /// <summary>
        /// Retrieves a list of user's groups with pagination.
        /// </summary>
        [Cache(1000)]
        [HttpGet("groups")]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<GroupDto>>> GetOwnGroupsAsync([FromQuery] GroupParams groupParams)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401, "User not authenticated."));

            groupParams.StudentId = user.Id;
            var spec = new GroupSpecification(groupParams);
            var groups = await _groupService.GetGroupsAsync(spec) ?? new List<Group>();
            var countSpec = new GroupSpecification(groupParams, false);
            var totalGroups = (await _groupService.GetGroupsAsync(countSpec)).Count();

            var groupDtos = groups.Any() ? groups.Select(g => g.ToDto()).ToList() : new List<GroupDto?>();
            var pagination = new Pagination<GroupDto>(groupParams.PageIndex, groupParams.PageSize, totalGroups, groupDtos);
            return Ok(pagination);
        }

    }
}