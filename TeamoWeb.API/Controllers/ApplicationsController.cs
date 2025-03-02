using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Teamo.Core.Entities;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Applications;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{   
    [Route("api/groups/{groupId}/applications")]
    public class ApplicationsController : BaseApiController
    {
        private readonly IApplicationService _appService;
        private readonly INotificationService _notiService;
        private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;

        public ApplicationsController(
            IApplicationService appService, 
            IUserService userService, 
            INotificationService notiService,
            IDeviceService deviceService
        )
        {
            _appService = appService;
            _userService = userService;
            _notiService = notiService;
            _deviceService = deviceService;
        }

        //Get application by id
        [HttpGet("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApplicationDto?>> GetApplicationById(int id)
        {
            var app = await _appService.GetApplicationByIdAsync(id);
            if(app == null) return NotFound(new ApiErrorResponse(404, "Application not found"));
            return Ok(app.ToDto());
        }

        //Get group's applications with spec
        [HttpGet]
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
        [HttpPatch("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> ReviewApplication(int groupId, int id, [FromBody] ApplicationToUpsertDto appDto)
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
            var app = await _appService.GetApplicationByIdAsync(id);
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
                FCMessage message = CreateApplicationReviewMessage(deviceTokens, groupId, id, user, status);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult) 
                    return Ok(new ApiErrorResponse(200, 
                        "Application reviewed successfully, " +
                        "but some failed to send notifications to some devices."));
            }

            return Ok(new ApiErrorResponse(200, "Application reviewed successfully."));
        }

        //Create and send an application
        [HttpPost]
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
        [HttpDelete("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> DeleteApplication(int id)
        {
            var app = await _appService.GetApplicationByIdAsync(id);
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


        //Get user's sent applications with spec
        [HttpGet("/api/applications")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IReadOnlyList<ApplicationDto>>> GetSentApplications([FromQuery] ApplicationParams appParams)
        {
            var user = await _userService.GetUserByClaims(HttpContext.User);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorized"));
            
            appParams.StudentId = user.Id;

            var appSpec = new ApplicationSpecification(appParams);
            var apps = await _appService.GetSentApplicationsAsync(appSpec);
            var count = await _appService.CountAsync(appSpec);
            var appsToDto = apps.Select(a => a.ToDto()).ToList();
            var pagination = new Pagination<ApplicationDto>(appParams.PageIndex,appParams.PageSize,count,appsToDto);
            return Ok(pagination);
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