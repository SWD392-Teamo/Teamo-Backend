using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public GroupsController(
            IGroupService groupService, 
            IUserService userService,
            IUploadService uploadService,
            IConfiguration config
        )
        {
            _groupService = groupService;
            _userService = userService;
            _uploadService = uploadService;
            _config = config;
        }

        /// <summary>
        /// Retrieves a list of groups with pagination.
        /// </summary>
        [Cache(1000)]
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
        [Cache(1000)]
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
        [InvalidateCache("/groups")]
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<GroupDto>> CreateGroupAsync(GroupToUpsertDto groupDto, [FromForm] IFormFile image)
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
        [InvalidateCache("/groups")]
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

        [InvalidateCache("/groups")]
        [HttpPost("{id}/image")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UploadGroupImage(int id, [FromForm] IFormFile image) 
        {
            // Check if an image was chosen
            if (image == null) return BadRequest(new ApiErrorResponse(400, "No image found"));

            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return BadRequest(new ApiErrorResponse(404, "Group not found!"));

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

            await _groupService.DeleteGroupAsync(group);
            return Ok("Successfully deleted a group");
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

            var groupMember = groupMemberToAddDto.ToEntity();
            groupMember.GroupId = id;
            await _groupService.AddMemberToGroup(groupMember);

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
            var groupMember = await _groupService.GetGroupMemberAsync(groupId, studentId);
            if (groupMember == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group Member not found!"));
            }
            await _groupService.RemoveMemberFromGroup(groupMember);
            return Ok("Delete Successfully");
        }

        [InvalidateCache("/groups")]
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
