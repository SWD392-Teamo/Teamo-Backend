using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Users;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{
    //Managing users (students)
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
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
            if (user == null) return NotFound();
            return Ok(user.ToProfileDto());
        }

        //Ban user
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProfileDto>> BanUser (int id)
        {
            var userSpec = new UserSpecification(id);
            var user = await _userService.GetUserWithSpec(userSpec);
            if(user == null) return NotFound();

            if(user.Status == UserStatus.Active) return BadRequest();
            user.Status = UserStatus.Inactive;
            
            var result = await _userService.UpdateUserAsync(user);
            if (result.Succeeded) return Ok();
            else return BadRequest();
        }
    }
}