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
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<ProfileDto>>> GetUsers([FromQuery] UserSpecParams userSpecParams)
        {
            var userSpec = new UserSpecification(userSpecParams);
            var userCountSpec = new UserSpecification(userSpecParams, false);
            var users = await _userService.ListUsersAsync(userSpec);
            var count = await _userService.CountAsync(userCountSpec);
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
            user.Status = UserStatus.Banned;
            
            var result = await _userService.UpdateUserAsync(user);
            
            if (result.Succeeded) return Ok(user.ToProfileDto());
            else return BadRequest(new ApiErrorResponse(400, "Failed to ban user."));
        }

        [InvalidateCache("/users")]
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProfileDto>> UnbanUser(int id)
        {
            var userSpec = new UserSpecification(id);
            var user = await _userService.GetUserWithSpec(userSpec);
            if (user == null) return NotFound(new ApiErrorResponse(404, "User not found."));

            user.Status = UserStatus.Active;

            var result = await _userService.UpdateUserAsync(user);

            if (result.Succeeded) return Ok(user.ToProfileDto());
            else return BadRequest(new ApiErrorResponse(400, "Failed to ban user."));
        }
    }
}