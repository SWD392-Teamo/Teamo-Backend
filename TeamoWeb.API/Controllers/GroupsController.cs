﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces.Services;
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
        private readonly IUploadService _uploadService;
        private readonly IConfiguration _config;
        private readonly INotificationService _notiService;
        private readonly IDeviceService _deviceService;

        public GroupsController(
            IGroupService groupService, 
            IUserService userService,
            IUploadService uploadService,
            IConfiguration config,
            INotificationService notiService,
            IDeviceService deviceService
        )
        {
            _groupService = groupService;
            _userService = userService;
            _uploadService = uploadService;
            _config = config;
            _notiService = notiService;
            _deviceService = deviceService;
        }

        /// <summary>
        /// Retrieves a list of groups with pagination.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<GroupDto>>> GetGroupsAsync([FromQuery] GroupParams groupParams)
        {
            var isAdmin = HttpContext.User.IsInRole(UserRole.Admin.ToString());
            var spec = new GroupSpecification(groupParams, isAdmin: isAdmin);
            var groups = await _groupService.GetGroupsAsync(spec) ?? new List<Group>();
            var count = await _groupService.CountGroupsAsync(spec);
            var groupDtos = groups.Any() ? groups.Select(g => g.ToDto()).ToList() : new List<GroupDto?>();
            var pagination = new Pagination<GroupDto>(groupParams.PageIndex, groupParams.PageSize, count, groupDtos);
            return Ok(pagination);
        }       

        /// <summary>
        /// Retrieves group details by ID.
        /// </summary>
        [Cache(1000)]
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetGroupByIdAsync(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null) return NotFound(new ApiErrorResponse(404, "Group not found.")); 
            return Ok(group.ToDto());
        }

        /// <summary>
        /// Creates a new group 
        /// </summary>
        [InvalidateCache("/groups")]
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<GroupDto>> CreateGroupAsync(GroupToUpsertDto groupDto)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorize"));

            var group = groupDto.ToEntity();

            var result = await _groupService.CreateGroupAsync(group, user.Id);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to create group."));

            group = await _groupService.GetGroupByIdAsync(group.Id);
            var createdGroupDto = group.ToDto();
            return Ok(createdGroupDto);
        }

        /// <summary>
        /// Updates an existing group by ID.
        /// </summary>
        [InvalidateCache("/groups")]
        [HttpPatch("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<GroupDto>> UpdateGroupAsync(int id, GroupToUpsertDto groupDto)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound(new ApiErrorResponse(404, "Group not found!"));

            var user = await _userService.GetUserByClaims(HttpContext.User);
            var isLeader = await _groupService.CheckGroupLeaderAsync(id, user.Id);
            if (!isLeader) return Unauthorized(new ApiErrorResponse(401, "Only group leader can update group."));

            var updatedGroup = groupDto.ToEntity(group);
            var result = await _groupService.UpdateGroupAsync(updatedGroup);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to update group."));

            updatedGroup = await _groupService.GetGroupByIdAsync(group.Id);

            var groupMembers = await _groupService.GetAllGroupMembersAsync(group.Id);
            var groupMembersIds = groupMembers.Select(g => g.StudentId).ToList();

            // Get all members' devices
            var deviceTokens = await _deviceService.GetDeviceTokensForSelectedUsers(groupMembersIds);

            if (!deviceTokens.IsNullOrEmpty())
            {
                var status = updatedGroup.Status.ToString().ToLower();

                // Generate notification contents
                FCMessage message = CreateUpdatedGroupMessage(deviceTokens, updatedGroup, status);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult) 
                    return Ok(new ApiErrorResponse(200, 
                        "Group updated successfully, " +
                        "but failed to send notifications to some devices."));
            }

            var updatedGroupDto = updatedGroup.ToDto();
            return Ok(updatedGroupDto);
        }

        [InvalidateCache("/groups")]
        [HttpPost("{id}/images")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UploadGroupImage(int id, IFormFile image) 
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            var isLeader = await _groupService.CheckGroupLeaderAsync(id, user.Id);
            if (!isLeader) return Unauthorized(new ApiErrorResponse(401, "Only group leader can update group."));
            
            // Check if an image was chosen
            if (image == null) return BadRequest(new ApiErrorResponse(400, "No image found"));

            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound(new ApiErrorResponse(404, "Group not found!"));

            // Upload and get download Url
            var imgUrl = await _uploadService.UploadFileAsync(
                image.OpenReadStream(), 
                image.FileName, 
                image.ContentType,
                _config["Firebase:GroupImagesUrl"]);

            // Update image url
            group.ImgUrl = imgUrl;

            var result = await _groupService.UpdateGroupAsync(group);
            
            if (!result) return BadRequest(new ApiErrorResponse(400, "Failed to upload image."));

            return Ok(new ApiErrorResponse(200, "Image uploaded successfully.", imgUrl));
        }

        /// <summary>
        /// Deletes a group by ID.
        /// </summary>
        [InvalidateCache("/groups")]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<GroupDto>> DeleteGroupAsync(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound(new ApiErrorResponse(404, "Group not found!"));

            var user = await _userService.GetUserByClaims(HttpContext.User);
            var isLeader = await _groupService.CheckGroupLeaderAsync(id, user.Id);
            if (!isLeader) return Unauthorized(new ApiErrorResponse(401, "Only group leader can delete group."));

            var result = await _groupService.DeleteGroupAsync(group);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to delete group."));

            var groupMembers = await _groupService.GetAllGroupMembersAsync(id);
            var groupMembersIds = groupMembers.Select(g => g.StudentId).ToList();

            // Get all members' devices
            var deviceTokens = await _deviceService.GetDeviceTokensForSelectedUsers(groupMembersIds);

            if (!deviceTokens.IsNullOrEmpty())
            {
                var status = group.Status.ToString().ToLower();

                // Generate notification contents
                FCMessage message = CreateDeletedGroupMessage(deviceTokens, group.Name, group.Id, status);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult) 
                    return Ok(new ApiErrorResponse(200, 
                        "Group deleted successfully, " +
                        "but failed to send notifications to some devices."));
            }

            return Ok(new ApiErrorResponse(200, "Successfully deleted group"));
        }

        [InvalidateCache("/groups")]
        [HttpPatch("{id}/ban")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GroupDto>> BanGroupAsync(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound(new ApiErrorResponse(404, "Group not found!"));

            var result = await _groupService.BanGroupAsync(group);
            if (!result) return BadRequest(new ApiErrorResponse(400, "Failed to ban group."));
            
            var groupMembers = await _groupService.GetAllGroupMembersAsync(id);
            var groupMembersIds = groupMembers.Select(g => g.StudentId).ToList();

            // Get all members' devices
            var deviceTokens = await _deviceService.GetDeviceTokensForSelectedUsers(groupMembersIds);

            if (!deviceTokens.IsNullOrEmpty())
            {
                var status = group.Status.ToString().ToLower();

                // Generate notification contents
                FCMessage message = CreateBanUnbanGroupMessage(deviceTokens, group.Name, group.Id, status);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult)
                    return Ok(new ApiErrorResponse(200,
                        "Group banned successfully, " +
                        "but failed to send notifications to some devices."));
            }

            return Ok(group.ToDto());
        }
        [InvalidateCache("/groups")]
        [HttpPatch("{id}/unban")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GroupDto>> UnBanGroupAsync(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound(new ApiErrorResponse(404, "Group not found!"));

            var result = await _groupService.UnBanGroupAsync(group);
            if (!result) return BadRequest(new ApiErrorResponse(400, "Failed to ban group."));

            var groupMembers = await _groupService.GetAllGroupMembersAsync(id);
            var groupMembersIds = groupMembers.Select(g => g.StudentId).ToList();

            // Get all members' devices
            var deviceTokens = await _deviceService.GetDeviceTokensForSelectedUsers(groupMembersIds);

            if (!deviceTokens.IsNullOrEmpty())
            {
                var status = group.Status.ToString().ToLower();

                // Generate notification contents
                FCMessage message = CreateBanUnbanGroupMessage(deviceTokens, group.Name, group.Id, status);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult)
                    return Ok(new ApiErrorResponse(200,
                        "Group banned successfully, " +
                        "but failed to send notifications to some devices."));
            }

            return Ok(group.ToDto());
        }

        /// <summary>
        /// Adds a member to a group.
        /// </summary>
        [InvalidateCache("/groups")]
        [HttpPost("{id}/members")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AddMemberToGroup(int id, GroupMemberToAddDto groupMemberToAddDto)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound(new ApiErrorResponse(404, "Group not found!"));

            var user = await _userService.GetUserByClaims(HttpContext.User);
            var isLeader = await _groupService.CheckGroupLeaderAsync(id, user.Id);
            if (!isLeader) return Unauthorized(new ApiErrorResponse(401, "Only group leader can add members."));
            
            var groupMember = groupMemberToAddDto.ToEntity();
            groupMember.GroupId = id;
            var result = await _groupService.AddMemberToGroup(groupMember);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to add member to group."));

            groupMember = await _groupService.GetGroupMemberAsync(groupMember.GroupId, groupMember.StudentId);
            return Ok(groupMember.ToDto());
        }

        /// <summary>
        /// Removes a member from a group.
        /// </summary>
        [InvalidateCache("/groups")]
        [HttpDelete("{groupId}/members/{studentId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RemoveMemberFromGroup(int groupId, int studentId)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            var isLeader = await _groupService.CheckGroupLeaderAsync(groupId, user.Id);
            if (!isLeader) return Unauthorized(new ApiErrorResponse(401, "Only group leader can remove members."));
            
            var groupMember = await _groupService.GetGroupMemberAsync(groupId, studentId);
            if (groupMember == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group Member not found!"));
            }

            var result = await _groupService.RemoveMemberFromGroup(groupMember);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to remove member from group."));

            var group = await _groupService.GetGroupByIdAsync(groupId);
            var groupName = group.Name;

            // Get removed member's devices
            var deviceTokens = await _deviceService.GetDeviceTokensForUser(studentId);

            if (!deviceTokens.IsNullOrEmpty())
            {
                // Generate notification contents
                FCMessage message = CreateRemovedMemberMessage(deviceTokens, groupId, groupName);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult) 
                    return Ok(new ApiErrorResponse(200, 
                        "Member removed successfully, " +
                        "but failed to send notifications to some devices."));
            }

            return Ok(new ApiErrorResponse(200, "Removed member successfully."));
        }

        [InvalidateCache("/groups")]
        [HttpPatch("{groupId}/members/{studentId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateGroupMember (int groupId, int studentId, GroupMemberToAddDto gmDto)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            var isLeader = await _groupService.CheckGroupLeaderAsync(groupId, user.Id);
            if (!isLeader) return Unauthorized(new ApiErrorResponse(401, "Only group leader can update members."));
            
            var groupMember = await _groupService.GetGroupMemberAsync(groupId, studentId);
            if (groupMember == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group Member not found!"));
            }
            groupMember = gmDto.ToEntity(groupMember);
            var result = await _groupService.UpdateGroupMemberAsync(groupMember);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to update group member."));

            var group = await _groupService.GetGroupByIdAsync(groupId);
            var groupName = group.Name;

            // Get updated member's devices
            var deviceTokens = await _deviceService.GetDeviceTokensForUser(studentId);

            if (!deviceTokens.IsNullOrEmpty())
            {
                // Generate notification contents
                FCMessage message = CreateUpdatedMemberMessage(deviceTokens, groupId, groupName);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult) 
                    return Ok(new ApiErrorResponse(200, 
                        "Member updated successfully, " +
                        "but failed to send notifications to some devices."));
            }

            return Ok(groupMember.ToDto());
        }

        private static FCMessage CreateUpdatedGroupMessage(List<string> tokens, 
            Group group, string status)
        {
            return new FCMessage
            {
                tokens = tokens,
                title = "Group Update",
                body = $"{group.Name} has been updated recently.",
                data = new Dictionary<string, string>
                {
                    { "type", "updated_group" },
                    { "groupId", group.Id.ToString() },
                    { "status", status }
                }
            };
        }

        private static FCMessage CreateDeletedGroupMessage(List<string> tokens, 
            string groupName, int groupId, string status)
        {
            return new FCMessage
            {
                tokens = tokens,
                title = "Delete group",
                body = $"{groupName} has been deleted.",
                data = new Dictionary<string, string>
                {
                    { "type", "deleted_group" },
                    { "groupName", groupName },
                    { "groupId", groupId.ToString() },
                    { "status", status }
                }
            };
        }
        private static FCMessage CreateBanUnbanGroupMessage(List<string> tokens,
            string groupName, int groupId, string status)
        {
            bool isBanned = status == GroupStatus.Banned.ToString().ToLower();
            return new FCMessage
            {
                tokens = tokens,
                title = $"{(isBanned ? "Ban" : "Unban")} group",
                body = isBanned
                        ? $"The group '{groupName}' has been banned due to policy violations. Please contact the administrator for further assistance."
                        : $"The group '{groupName}' has been unbanned and is now active again. You can continue using all group features.",
                data = new Dictionary<string, string>
                {
                    { "type", $"{(isBanned ? "banned" : "unbanned")}_group" },
                    { "groupName", groupName },
                    { "groupId", groupId.ToString() },
                    { "status", status}
                }
            };
        }

        private static FCMessage CreateRemovedMemberMessage(List<string> tokens, 
            int groupId, string groupName)
        {
            return new FCMessage
            {
                tokens = tokens,
                title = "Member Removal",
                body = $"You have been removed from {groupName}",
                data = new Dictionary<string, string>
                {
                    { "type", "removed_member" },
                    { "groupId", groupId.ToString() },
                    { "groupName", groupName }
                }
            };
        }

        private static FCMessage CreateUpdatedMemberMessage(List<string> tokens, 
            int groupId, string groupName)
        {
            return new FCMessage
            {
                tokens = tokens,
                title = "Member Update",
                body = $"Your positions in {groupName} have been updated.",
                data = new Dictionary<string, string>
                {
                    { "type", "updated_member" },
                    { "groupId", groupId.ToString() },
                    { "groupName", groupName }
                }
            };
        }
    }
}
