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
        private readonly IApplicationService _appService;
        private readonly INotificationService _notiService;
        private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;
        public GroupsController(
            IGroupService groupService, 
            IUserService userService, 
            IApplicationService appService, 
            INotificationService notiService, 
            IDeviceService deviceService
        )
        {
            _groupService = groupService;
            _userService = userService;
            _appService = appService;
            _notiService = notiService;
            _deviceService = deviceService;
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

            groupPosition = await _groupService.GetGroupPositionAsync(groupPosition.Id);
            return Ok(groupPosition.ToDto());

        }

        /// <summary>
        /// Update group position
        /// </summary>
        [HttpPatch("{groupId}/positions/{positionId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateGroupPosition(int groupId, int positionId, GroupPositionToUpsertDto updateDto)
        {           
            var groupPosition = await _groupService.GetGroupPositionAsync(positionId);
            if (groupPosition == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group position not found."));
            }

            groupPosition = updateDto.ToEntity(groupPosition);
            groupPosition.GroupId = groupId;
            await _groupService.UpdateGroupPositionAsync(groupPosition, updateDto.SkillIds);

            groupPosition = await _groupService.GetGroupPositionAsync(positionId);
            return Ok(groupPosition.ToDto());
        }

        /// <summary>
        /// Removes a group position.
        /// </summary>
        [HttpDelete("{groupId}/positions/{positionId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> DeleteGroupPosition(int positionId)
        {
            var groupPosition = await _groupService.GetGroupPositionAsync(positionId);
            if (groupPosition == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group position not found."));
            }

            await _groupService.RemoveGroupPositionAsync(groupPosition);
            return Ok("Deleted successfully");
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
            await _groupService.UpdateGroupMemberAsync(groupMember, gmDto.GroupPositionIds);

            groupMember = await _groupService.GetGroupMemberAsync(groupId, studentId);
            return Ok(groupMember.ToDto());
        }

        //Get group's applications with spec
        [HttpGet("{groupId}/applications")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IReadOnlyList<ApplicationDto>>> GetGroupApplications(int groupId, [FromQuery] ApplicationParams appParams)
        {
            appParams.GroupId = groupId;

            //Check if current user is the leader of corresponding group
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));

            var groupLeaderId = await _appService.GetGroupLeaderIdAsync(groupId);
            if(user.Id != groupLeaderId) 
                return BadRequest(new ApiErrorResponse(400, "Only group leaders can view applications"));

            var appSpec = new ApplicationGroupSpecification(appParams);
            var apps = await _appService.GetGroupApplicationsAsync(appSpec);
            var count = await _appService.CountAsync(appSpec);
            var appsToDto = apps.Select(a => a.ToDto()).ToList();
            var pagination = new Pagination<ApplicationDto>(appParams.PageIndex,appParams.PageSize,count,appsToDto);
            return Ok(pagination);
        }

        //Approve or reject application
        [HttpPatch("{groupId}/applications/{appId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> ReviewApplication(int groupId, int appId, [FromBody] ApplicationToUpsertDto appDto)
        {
            // Get current user
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));

            // Check if current user is the leader of corresponding group
            var groupLeaderId = await _appService.GetGroupLeaderIdAsync(groupId);
            if(user.Id != groupLeaderId) 
                return BadRequest(new ApiErrorResponse(400, "Only group leaders can review applications."));
            

            // Get the application to review
            var app = await _appService.GetApplicationByIdAsync(appId);
            if(app == null || app.Status != ApplicationStatus.Requested) 
                return BadRequest(new ApiErrorResponse(400, "Unable to review this application."));

            appDto.ToEntity(app);

            // Update status
            var result = await _appService.ReviewApplicationAsync(app);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to review application."));
            
            // Get all current user's devices
            var deviceTokens = await _deviceService.GetDeviceTokensForUser(app.StudentId);

            if (!deviceTokens.IsNullOrEmpty())
            {
                var status = app.Status.ToString().ToLower();

                // Generate notification contents
                FCMessage message = CreateApplicationReviewMessage(deviceTokens, groupId, appId, user, status);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult) 
                    return Ok(new ApiErrorResponse(200, 
                        "Application reviewed successfully, " +
                        "but some failed to send notifications to some devices."));
            }

            return Ok(new ApiErrorResponse(200, "Application reviewed successfully."));
        }

        //Create and send an application
        [HttpPost("{groupId}/applications")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApplicationDto>> CreateNewApplication(int groupId, [FromBody] ApplicationToUpsertDto appDto)     
        {            
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            
            appDto.StudentId = user.Id;
            appDto.GroupId = groupId;

            //Check for validity to apply
            var isValid = await _appService.CheckValidToApply(appDto.GroupId,appDto.StudentId,appDto.GroupPositionId);
            if(!isValid) return BadRequest(new ApiErrorResponse(400, "Not applicable to create application."));

            var app = appDto.ToEntity();
            
            var result = await _appService.CreateNewApplicationAsync(app);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to create and send application."));
            return Ok(new ApiErrorResponse(200, "Application sent successfully."));
        }

        //Delete application (recall unanswered application)
        [HttpDelete("{groupId}/applications/{appId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> DeleteApplication(int appId)
        {
            var app = await _appService.GetApplicationByIdAsync(appId);
            if(app == null) return NotFound(new ApiErrorResponse(404, "Application not found."));

            //Check if current user is the sender of the corresponding application
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null || user.Id != app.StudentId)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            
            if(app.Status != ApplicationStatus.Requested)
                return BadRequest(new ApiErrorResponse(400, "The application has been reviewed."));

            var result = await _appService.DeleteApplicationAsync(app);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to delete application."));
            return Ok(new ApiErrorResponse(200, "Application deleted successfully."));
        }

        private static FCMessage CreateApplicationReviewMessage(List<string> tokens, 
            int groupId, int appId, User user, string status)
        {
            return new FCMessage
            {
                tokens = tokens,
                title = "Application Update",
                body = $"Your application has been {status} by {user.FirstName + " " + user.LastName}",
                data = new Dictionary<string, string>
                {
                    { "type", "application_review" },
                    { "groupId", groupId.ToString() },
                    { "applicationId", appId.ToString() },
                    { "status", status }
                }
            };
        }
    }
}
