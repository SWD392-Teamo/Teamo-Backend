using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Interfaces.Services;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;


namespace TeamoWeb.API.Controllers
{
    [Route("api/groups/{groupId}/positions")]
    public class PositionController : BaseApiController
    {
        private readonly IGroupService _groupService;
        public PositionController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        /// <summary>
        /// Add position to group
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AddGroupPosition([FromRoute]int groupId, GroupPositionToUpsertDto groupPositionDto)
        {
            var group = await _groupService.GetGroupByIdAsync(groupId);
            if (group == null)
                return NotFound(new ApiErrorResponse(404, "Group not found!"));

            var groupPosition = groupPositionDto.ToEntity();
            groupPosition.GroupId = groupId;
            await _groupService.AddGroupPosition(groupPosition);

            groupPosition = await _groupService.GetGroupPositionAsync(groupPosition.Id);
            return Ok(groupPosition.ToDto());

        }
        /// <summary>
        /// Update group position
        /// </summary>
        [HttpPatch("{positionId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateGroupPosition([FromRoute]int groupId, int positionId, GroupPositionToUpsertDto updateDto)
        {
            var groupPosition = await _groupService.GetGroupPositionAsync(positionId);
            if (groupPosition == null)
            {
                return NotFound(new ApiErrorResponse(404, "Group position not found."));
            }

            groupPosition = updateDto.ToEntity(groupPosition);
            groupPosition.GroupId = groupId;
            await _groupService.UpdateGroupPositionAsync(groupPosition);

            groupPosition = await _groupService.GetGroupPositionAsync(positionId);
            return Ok(groupPosition.ToDto());
        }

        /// <summary>
        /// Removes a group position.
        /// </summary>
        [HttpDelete("{positionId}")]
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
    }
}
