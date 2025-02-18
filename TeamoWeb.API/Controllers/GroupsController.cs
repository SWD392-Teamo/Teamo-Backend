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

        /// <summary>
        /// Retrieves a list of groups with pagination.
        /// </summary>
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

        /// <summary>
        /// Retrieves group details by ID.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<GroupDto>> GetGroupByIdAsync(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null) return NotFound();
            return Ok(group.ToDto());
        }

        /// <summary>
        /// Creates a new group 
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<GroupDto>> CreateGroupAsync(GroupToUpsertDto groupDto)
        {

            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorize"));

            var group = groupDto.ToEntity();

            await _groupService.CreateGroupAsync(group, user.Id);
            group = await _groupService.GetGroupByIdAsync(group.Id);
            var createdGroupDto = group.ToDto();
            return Ok(createdGroupDto);

        }

        /// <summary>
        /// Updates an existing group by ID.
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<GroupDto>> UpdateGroupAsync(int id, GroupToUpsertDto groupDto)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return BadRequest(new ApiErrorResponse(404, "Group not found!"));

            var updatedGroup = groupDto.ToEntity(group);
            await _groupService.UpdateGroupAsync(updatedGroup);
            updatedGroup = await _groupService.GetGroupByIdAsync(group.Id);
            var updatedGroupDto = updatedGroup.ToDto();
            return Ok(updatedGroupDto);
        }

        /// <summary>
        /// Deletes a group by ID.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<GroupDto>> DeleteGroupAsync(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound(new ApiErrorResponse(404, "Group not found!"));

            await _groupService.DeleteGroupAsync(group);
            return Ok(new ApiErrorResponse(200, "Successfully deleted a group!"));
        }

        /// <summary>
        /// Adds a member to a group.
        /// </summary>
        [HttpPost("{id}/members")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AddMemberToGroup(int id, GroupMemberToAddDto groupMemberToAddDto)
        {

            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound(new ApiErrorResponse(404, "Group not found!"));

            var groupMember = groupMemberToAddDto.ToEntity();
            groupMember.GroupId = id;
            await _groupService.AddMemberToGroup(groupMember);

            group = await _groupService.GetGroupByIdAsync(id);
            return Ok(group.ToDto());
        }
        /// <summary>
        /// Add position to group
        /// </summary>
        [HttpPost("{id}/positions")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AddGroupPosition (int id, GroupPositionToUpsertDto groupPositionDto)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound(new ApiErrorResponse(404, "Group not found!"));

            var groupPosition = groupPositionDto.ToEntity();
            groupPosition.GroupId = id;
            await _groupService.AddGroupPosition(groupPosition);

            group = await _groupService.GetGroupByIdAsync(id);
            return Ok(group.ToDto());

        }

        /// <summary>
        /// Update group position
        /// </summary>
        [HttpPatch("{groupId}/positions/{positionId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateGroupPosition(int groupId, int positionId, GroupPositionToUpsertDto updateDto)
        {           
            var groupPosition = await _groupService.GetGroupPositionByIdAsync(positionId);
            if (groupPosition == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group position not found."));
            }

            groupPosition = updateDto.ToEntity(groupPosition);
            groupPosition.GroupId = groupId;
            await _groupService.UpdateGroupPositionAsync(groupPosition, updateDto.SkillIds);

            var group = await _groupService.GetGroupByIdAsync(groupId);
            return Ok(group.ToDto());
        }

        /// <summary>
        /// Removes a group position.
        /// </summary>
        [HttpDelete("{groupId}/positions/{groupPositionId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> DeleteGroupPosition(int groupPositionId)
        {
            var groupPosition = await _groupService.GetGroupPositionByIdAsync(groupPositionId);
            if (groupPosition == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group position not found."));
            }

            await _groupService.RemoveGroupPositionAsync(groupPosition);
            return Ok();
        }

        /// <summary>
        /// Removes a member from a group.
        /// </summary>
        [HttpDelete("{groupId}/members/{groupMemberId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RemoveMemberFromGroup(int groupId, int groupMemberId)
        {
            var groupMember = await _groupService.GetGroupMemberByIdAsync(groupMemberId);
            if (groupMember == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group Member not found!"));
            }
            await _groupService.RemoveMemberFromGroup(groupMember);

            var group = await _groupService.GetGroupByIdAsync(groupId);
            return Ok(group.ToDto());
        }

        [HttpPatch("{groupId}/members/{groupMemberId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateGroupMember (int groupId, int groupMemberId, GroupMemberToAddDto gmDto)
        {
            var groupMember = await _groupService.GetGroupMemberByIdAsync(groupMemberId);
            if (groupMember == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group Member not found!"));
            }
            groupMember = gmDto.ToEntity(groupMember);
            await _groupService.UpdateGroupMemberAsync(groupMember, gmDto.GroupPositionIds);

            var group = await _groupService.GetGroupByIdAsync(groupId);
            return Ok(group.ToDto());
        }
    }
}
