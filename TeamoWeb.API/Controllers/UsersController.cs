using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Teamo.Core.Entities;
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
        private readonly IDeviceService _deviceService;
        private readonly INotificationService _notiService;


        public UsersController(
            IUserService userService,
            IProfileService profileService,
            IUploadService uploadService,
            IConfiguration config,
            IDeviceService deviceService,
            INotificationService notiService)
        {
            _userService = userService;
            _profileService = profileService;
            _uploadService = uploadService;
            _config = config;
            _deviceService = deviceService;
            _notiService = notiService;
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
            var pagination = new Pagination<ProfileDto>(userSpecParams.PageIndex, userSpecParams.PageSize, count, usersToProfileDto);
            return Ok(pagination);
        }

        //Get user by id
        [Cache(1000)]
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult<ProfileDto?>> GetUserById(int id)
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
        public async Task<ActionResult<ProfileDto>> BanUser(int id)
        {
            var userSpec = new UserSpecification(id);
            var user = await _userService.GetUserWithSpec(userSpec);
            if (user == null) return NotFound(new ApiErrorResponse(404, "User not found."));

            if (user.Status != UserStatus.Active) return BadRequest(new ApiErrorResponse(400, "Failed to ban user."));
            user.Status = UserStatus.Banned;

            var result = await _userService.UpdateUserAsync(user);
            if (!result.Succeeded) BadRequest(new ApiErrorResponse(400, "Failed to unban user."));
            // Get all members' devices
            var deviceTokens = await _deviceService.GetDeviceTokensForUser(user.Id);

            if (!deviceTokens.IsNullOrEmpty())
            {
                var status = user.Status.ToString().ToLower();

                // Generate notification contents
                FCMessage message = CreateBanUnbanUserMessage(deviceTokens, user.Id, status);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult)
                    return Ok(new ApiErrorResponse(200,
                        "User unbanned successfully, " +
                        "but failed to send notifications to some devices."));
            }

            return Ok(user.ToProfileDto());
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
            if (!result.Succeeded) BadRequest(new ApiErrorResponse(400, "Failed to ban user."));

            // Get all members' devices
            var deviceTokens = await _deviceService.GetDeviceTokensForUser(user.Id);

            if (!deviceTokens.IsNullOrEmpty())
            {
                var status = user.Status.ToString().ToLower();

                // Generate notification contents
                FCMessage message = CreateBanUnbanUserMessage(deviceTokens, user.Id, status);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult)
                    return Ok(new ApiErrorResponse(200,
                        "User banned successfully, " +
                        "but failed to send notifications to some devices."));
            }
            
            return Ok(user.ToProfileDto());
        }

        private static FCMessage CreateBanUnbanUserMessage(List<string> tokens,int userId, string status)
        {
            bool isBanned = status == UserStatus.Banned.ToString().ToLower();
            return new FCMessage
            {
                tokens = tokens,
                title = $"{(isBanned ? "Ban" : "Unban")} user",
                body = isBanned
                        ? $"You have been banned due to policy violations. Please contact the administrator for further assistance."
                        : $"You have has been unbanned and is now active again. You can continue using all group features.",
                data = new Dictionary<string, string>
                {
                    { "type", $"{(isBanned ? "banned" : "unbanned")}_user" },
                    { "userId", userId.ToString() },
                    { "status", status}
                }

            };
        }
    
    }
}