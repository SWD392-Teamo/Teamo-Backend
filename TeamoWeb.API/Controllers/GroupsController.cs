using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Text.RegularExpressions;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications;
using Teamo.Core.Specifications.Groups;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;
using Group = Teamo.Core.Entities.Group;

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
                group = await _groupService.GetGroupByIdAsync(group.Id);
                var createdGroupDto = group.ToDto();
                return Ok(createdGroupDto);
            }
            catch (Exception ex) 
            {
                return BadRequest(new ApiErrorResponse(400, ex.Message, ex.InnerException?.Message));
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<GroupDto>> UpdateGroupAsync(int id, GroupToUpsertDto groupDto)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return BadRequest(new ApiErrorResponse(404, "Group not found!"));

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
                return BadRequest(new ApiErrorResponse(400, ex.Message, ex.InnerException?.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<GroupDto>> DeleteGroupAsync(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound(new ApiErrorResponse(404, "Group not found!"));

            try
            {
                await _groupService.DeleteGroupAsync(group);
                return Ok(new ApiErrorResponse(200, "Successfully deleted a group!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.Message, ex.InnerException?.Message));
            }
        }

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<GroupDto>> GetGroupByMemberIdAsync()
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorize"));

            var groupMemberParams = new GroupMemberParams
            {
                Studentd = user.Id
            };
            var spec = new GroupMemberSpecification(groupMemberParams);
            var groups = await _groupService.GetGroupsByMemberIdAsync(spec);
            var groupDtos = groups.Any() ? groups.Select(g => g.ToDto()).ToList() : new List<GroupDto?>();
            var pagination = new Pagination<GroupDto>(groupMemberParams.PageIndex, groupMemberParams.PageSize, groups.Count(), groupDtos);
            return Ok(pagination);
        }

        [HttpPost("{id}/members")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AddMemberToGroup(int id, GroupMemberToAddDto groupMemberToAddDto)
        {
            
            try
            {
                var groupMember = groupMemberToAddDto.ToEntity();
                groupMember.GroupId = id;
                await _groupService.AddMemberToGroup(groupMember);
                return Ok(new ApiErrorResponse(200, "Successfully add new member to group!"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiErrorResponse(409, ex.Message, ex.InnerException?.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.Message, ex.InnerException?.Message));
            }
        }

        [HttpPost("{id}/positions")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AddGroupPosition (int id, GroupPositionToAddDto groupPositionDto)
        {
            try
            {
                var groupPosition = groupPositionDto.ToEntity();
                groupPosition.GroupId = id;
                await _groupService.AddGroupPosition(groupPosition);
                return Ok(new ApiErrorResponse(200, "Successfully add position to group!"));
            }
            catch(Exception ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.Message, ex.InnerException?.Message));
            }
        }

        [HttpPatch("{groupId}/positions/{positionId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateGroupPosition(int groupId, int positionId, GroupPositionToAddDto updateDto)
        {
            var groupPosition = await _groupService.GetGroupPositionByIdAsync(positionId);
            if (groupPosition == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group position not found."));
            }

            try
            {
                groupPosition = updateDto.ToEntity(groupPosition);
                groupPosition.GroupId = groupId;
                await _groupService.UpdateGroupPositionAsync(groupPosition, updateDto.SkillIds);
                return Ok(new ApiErrorResponse(200, "Successfully update group position!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.Message, ex.InnerException?.Message));
            }
        }

        [HttpDelete("{groupId}/members/{groupMemberId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RemoveMemberFromGroup(int groupId, int groupMemberId)
        {
            var groupMember = await _groupService.GetGroupMemberByIdAsync(groupMemberId);
            if (groupMember == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group Member not found!"));
            }

            try
            {
                await _groupService.RemoveMemberFromGroup(groupMember);
                return Ok(new ApiErrorResponse(200, "Successfully remove member from group!"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiErrorResponse(409, ex.Message, ex.InnerException?.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.Message, ex.InnerException?.Message));
            }
        }
    }
}
