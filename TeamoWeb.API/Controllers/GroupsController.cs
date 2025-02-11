using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications;
using Teamo.Core.Specifications.Groups;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : BaseApiController
    {
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        public GroupsController(IGroupService groupService, IUserService userService)
        {
            _groupService = groupService;
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<GroupDto>>> GetGroupsAsync([FromQuery] GroupParams groupParams)
        {
            var spec = new GroupSpecification(groupParams);
            var groups = await _groupService.GetGroupsAsync(spec) ?? new List<Group>();
            var groupDtos = groups.Any() ? groups.Select(g => g.ToDto()).ToList() : new List<GroupDto?>();
            var pagination = new Pagination<GroupDto>(groupParams.PageIndex, groupParams.PageSize, groups.Count(), groupDtos);
            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<GroupDto>> GetGroupByIdAsync(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null) return NotFound();
            return Ok(group.ToDto());
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<GroupDto>> CreateGroupAsync(GroupToUpsertDto groupDto)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorize"));

            try
            {
                var group = groupDto.ToEntity();
                await _groupService.CreateGroupAsync(group, user.Id);
                var createdGroup = await _groupService.GetGroupByIdAsync(group.Id);
                var createdGroupDto = createdGroup.ToDto();
                return Ok(createdGroupDto);
            }
            catch (Exception ex) 
            {
                return BadRequest(new ApiErrorResponse(400, "Fail to create a group!", ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<GroupDto>> UpdateGroupAsync(int id, GroupToUpsertDto groupDto)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return BadRequest(new ApiErrorResponse(404, "This group does not exist!"));

            try
            {
                var updatedGroup = groupDto.ToEntity(group);
                await _groupService.UpdateGroupAsync(updatedGroup);
                updatedGroup = await _groupService.GetGroupByIdAsync(group.Id);
                var updatedGroupDto = updatedGroup.ToDto();
                return Ok(updatedGroupDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse(400, "Fail to update a group!", ex.Message));
            }
        }
    }
}
