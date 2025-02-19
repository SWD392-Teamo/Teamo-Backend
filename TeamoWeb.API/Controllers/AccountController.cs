using FirebaseAdmin.Auth;
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
        private readonly ITokenService _tokenService;

        public AccountController(IUserService userService, SignInManager<User> signInManager, 
            ITokenService tokenService) 
        {
            _userService = userService;
            _signInManager = signInManager;
            _tokenService = tokenService;
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

            var userRole = await _userService.GetUserRoleAsync(user);

            // Generate token and get expiry
            var (token, expires) = _tokenService.GenerateToken(user, userRole);

            return Ok(new 
            {
                userId = user.Id,
                Email = User.GetEmail(),
                Role = userRole,
                Token = token,
                Expires = expires
            });
        }

        [HttpPost("google-login")]
        public async Task<ActionResult> GoogleLogin([FromBody] GoogleLoginDto googleLoginDto)
        {
            try
            {
                // Get user email with firebase id token
                var email = await GetUserEmailFromGoogleProvider(googleLoginDto.IdToken);

                // Check if user exists
                var spec = new UserSpecification(email);
                var user = await _userService.GetUserWithSpec(spec);

                if (user == null)
                {
                    // Create new user
                    user = new User
                    {
                        Email = email,
                        UserName = UsernameGenerator(email),
                        EmailConfirmed = true,
                        Status = UserStatus.Active
                    };
                    var result = await _userService.CreateUserAsync(user);
                    if (!result.Succeeded) return BadRequest(result.Errors);

                    // Assign default role
                    await _userService.AddUserToRoleAsync(user, "Student");
                }

                // Check if account is inactive
                if (user.Status.Equals(UserStatus.Inactive.ToString()))
                    return Unauthorized();

                // Sign in the user with Identity
                await _signInManager.SignInAsync(user, true);

                var userRole = await _userService.GetUserRoleAsync(user);
                var (token, expires) = _tokenService.GenerateToken(user, userRole);

                return Ok(new
                {
                    userId = user.Id,
                    Email = email,
                    Role = userRole,
                    Token = token,
                    Expires = expires
                });
            }
            catch (FirebaseAuthException ex)
            {
                return Unauthorized(ex.Message);
            }
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

        private async Task<string> GetUserEmailFromGoogleProvider(string idToken)
        {
            // Verify the Firebase ID token
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                .VerifyIdTokenAsync(idToken);

            string uid = decodedToken.Uid;
            string email = decodedToken.Claims["email"].ToString();
            return email;
        }

        private string UsernameGenerator(string email)
        {
            string username = email.Split('@')[0];

            // Generate a random number between 1000 and 9999
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999);

            // Combine the username with the random number
            string uniqueUsername = $"{username}{randomNumber}";

            return uniqueUsername;
        }
    }
}
