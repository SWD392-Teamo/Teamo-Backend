using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Teamo.Core.Entities;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Applications;
using Teamo.Core.Specifications.Groups;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;
using Group = Teamo.Core.Entities.Group;

namespace TeamoWeb.API.Controllers
{
    public class GroupsController : BaseApiController
    {
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        public GroupsController(
            IGroupService groupService, 
            IUserService userService
        )
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
            return Ok("Successfully deleted a group");
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

            groupMember = await _groupService.GetGroupMemberAsync(groupMember.GroupId, groupMember.StudentId);
            return Ok(groupMember.ToDto());
        }

        /// <summary>
        /// Removes a member from a group.
        /// </summary>
        [HttpDelete("{groupId}/members/{studentId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RemoveMemberFromGroup(int groupId, int studentId)
        {
            var groupMember = await _groupService.GetGroupMemberAsync(groupId, studentId);
            if (groupMember == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group Member not found!"));
            }
            await _groupService.RemoveMemberFromGroup(groupMember);
            return Ok("Delete Successfully");
        }

        [HttpPatch("{groupId}/members/{studentId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateGroupMember (int groupId, int studentId, GroupMemberToAddDto gmDto)
        {
            var groupMember = await _groupService.GetGroupMemberAsync(groupId, studentId);
            if (groupMember == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group Member not found!"));
            }
            groupMember = gmDto.ToEntity(groupMember);
            await _groupService.UpdateGroupMemberAsync(groupMember);

            groupMember = await _groupService.GetGroupMemberAsync(groupId, studentId);
            return Ok(groupMember.ToDto());
        }
    }
}
