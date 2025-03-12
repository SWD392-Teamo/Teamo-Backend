using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Majors;
using Teamo.Core.Specifications.Students;
using Teamo.Core.Specifications.Users;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;

namespace TeamoWeb.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUserService userService, SignInManager<User> signInManager, 
            ITokenService tokenService, IUnitOfWork unitOfWork) 
        {
            _userService = userService;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var spec = new UserSpecification(loginDto.Email);

            var user = await _userService.GetUserWithSpec(spec);

            if (user == null) 
                return Unauthorized(new ApiErrorResponse(401, "Incorrect email"));

            // If account is closed then return unauthorized response
            if (user.Status.Equals(UserStatus.Inactive.ToString())) 
                return Unauthorized(new ApiErrorResponse(401, "User is banned, please contact the admin"));

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password!, loginDto.RememberMe, false);

            if (!result.Succeeded) 
                return Unauthorized(new ApiErrorResponse(401, "Incorrect password"));

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
                    // Get imported student using the email retrieved
                    var studentSpec = new StudentSpecification(email);
                    var importedStudent = await _unitOfWork.Repository<Student>().GetEntityWithSpec(studentSpec);

                    // Check if the user uses the correct email from FPT edu
                    if(importedStudent == null)
                    {
                        return Unauthorized(new ApiErrorResponse(401, "Please use the provided email from FPT education"));
                    }

                    // Get imported student's major code
                    var majorSpec = new MajorSpecification(importedStudent.MajorCode);
                    var major = await _unitOfWork.Repository<Major>().GetEntityWithSpec(majorSpec);

                    // Create new user
                    user = new User
                    {
                        Email = email,
                        UserName = email,
                        Code = importedStudent.Code,
                        FirstName = importedStudent.FirstName,
                        LastName = importedStudent.LastName,
                        Gender = importedStudent.Gender,
                        PhoneNumber = importedStudent.Phone,
                        MajorID = major.Id,
                        EmailConfirmed = true,
                        Status = UserStatus.Active
                    };
                    var result = await _userService.CreateUserAsync(user);
                    if (!result.Succeeded) return BadRequest(result.Errors);

                    // Assign default role
                    await _userService.AddUserToRoleAsync(user, UserRole.Student.ToString());
                }

                // Check if account is inactive
                if (user.Status.Equals(UserStatus.Inactive.ToString()))
                    return Unauthorized(new ApiErrorResponse(401, "User is banned, please contact the admin"));

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
    }
}
