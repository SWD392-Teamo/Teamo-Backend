using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Users;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Extensions;

namespace TeamoWeb.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly SignInManager<User> _signInManager;

        public AccountController(IUserService userService, SignInManager<User> signInManager) 
        {
            _userService = userService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var spec = new UserSpecification(loginDto.Email);

            var user = await _userService.GetUserWithSpec(spec);

            if (user == null) return Unauthorized();

            // If account is closed then return unauthorized response
            if (user.Status.Equals(UserStatus.Inactive.ToString())) return Unauthorized();

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password!, loginDto.RememberMe, false);

            if (!result.Succeeded) return Unauthorized();

            return Ok(new 
            {
                Email = User.GetEmail(),
                Role = User.GetRole(),
            });
            
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return NoContent();
        }

        [Authorize]
        [HttpGet("user-info")]
        public async Task<ActionResult<UserDto>> GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated == false) return Unauthorized();

            var spec = new UserSpecification(User.GetEmail());
            var user = await _userService.GetUserWithSpec(spec);

            var userDto = user.ToDto();
            if (userDto == null) return Unauthorized();
            
            userDto.Role = User.GetRole();
            return Ok(userDto);
        }

        [HttpGet("auth-status")]
        public ActionResult GetAuthState()
        {
            return Ok(new { IsAuthenticated = User.Identity?.IsAuthenticated ?? false });
        }
    }
}
