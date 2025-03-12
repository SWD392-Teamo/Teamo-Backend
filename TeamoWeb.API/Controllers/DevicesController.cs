using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;

namespace TeamoWeb.API.Controllers
{
    public class DevicesController : BaseApiController
    {
        private readonly IDeviceService _deviceService;

        public DevicesController(IDeviceService deviceService) 
        {
            _deviceService = deviceService;
        }

        [HttpPost("token")]
        [Authorize]
        public async Task<ActionResult> AddDeviceToken([FromBody] string token)
        {
            // Get currently logged in user id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Create a new user device
            var device = new UserDevice
            {
                UserId = int.Parse(userId),
                FCMToken = token,
            };

            var result = await _deviceService.AddDeviceAsync(device);

            if (!result) return BadRequest("Failed to add device");

            return Ok();
        }
    }
}
