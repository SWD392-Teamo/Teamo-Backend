using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Teamo.Core.Entities;

namespace TeamoWeb.API.Controllers
{
    public class TestController : BaseApiController
    {
        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized()
        {
            return Unauthorized();
        }

        [HttpGet("bad-request")]
        public IActionResult GetBadRequest()
        {
            return BadRequest("Not a good request");
        }

        [HttpGet("not-found")]
        public IActionResult GetNotFound()
        {
            return NotFound();
        }

        [HttpGet("internal-error")]
        public IActionResult GetInternalServerError()
        {
            throw new Exception("This is a test exception");
        }

        [HttpPost("validation-error")]
        public IActionResult GetValidationError(Major major)
        {
            return Ok();
        }

        // Test getting authorized user info
        [Authorize]
        [HttpGet("student-info")]
        public IActionResult GetSecret()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new
            {
                email
            });
        }

        // Test getting the admin info
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-info")]
        public IActionResult GetAdminSecret()
        {
            var isAdmin = User.IsInRole("Admin");
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var roles = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new
            {
                isAdmin,
                email,
                roles
            });
        }
    }
}
