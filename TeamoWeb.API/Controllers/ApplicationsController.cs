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
        private readonly IUploadService _uploadService;
        private readonly IConfiguration _config;

        public ApplicationsController(
            IApplicationService appService, 
            IUserService userService, 
            INotificationService notiService,
            IDeviceService deviceService,
            IUploadService uploadService,
            IConfiguration config
        )
        {
            _appService = appService;
            _userService = userService;
            _notiService = notiService;
            _deviceService = deviceService;
            _uploadService = uploadService;
            _config = config;
        }

        // Get application by id
        [Cache(1000)]
        [HttpGet("/api/applications/{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApplicationDto?>> GetApplicationById(int id)
        {
            var app = await _appService.GetApplicationByIdAsync(id);

            if (app == null) return NotFound(new ApiErrorResponse(404, "Application not found"));

            // Check if the student viewing the application is the
            // leader of the group that the application is sent to
            // or the student viewing the application is the one sent it
            var leaderId = await _appService.GetGroupLeaderIdAsync(app.GroupId);
            if (leaderId != User.GetId() && app.StudentId != User.GetId())
                return BadRequest(new ApiErrorResponse(400, "You are not allowed to view this application"));

            return Ok(app.ToDto());
        }

        // Get group's applications with spec
        [Cache(1000)]
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

        // Approve or reject application
        [InvalidateCache("/applications")]
        [HttpPatch("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApplicationDto>> ReviewApplication(int groupId, int id, [FromBody] ApplicationToUpsertDto appDto)
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

            app = appDto.ToEntity(app);

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
                        "but failed to send notifications to some devices."));
            }

            return Ok(app.ToDto());
        }

        // Create and send an application
        [InvalidateCache("/applications")]
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> CreateNewApplication(int groupId, [FromBody] ApplicationToUpsertDto appDto)     
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
            
            app = await _appService.CreateNewApplicationAsync(app);
            if(app == null) return BadRequest(new ApiErrorResponse(400, "Failed to create and send application."));

            var groupLeaderId = await _appService.GetGroupLeaderIdAsync(groupId);
            
            // Get group leader's devices
            var deviceTokens = await _deviceService.GetDeviceTokensForUser(groupLeaderId);

            if (!deviceTokens.IsNullOrEmpty())
            {
                // Generate notification contents
                FCMessage message = CreateNewApplicationMessage(deviceTokens, app.Group, app.Id);

                var notiResult = await _notiService.SendNotificationAsync(message);
                if (!notiResult) 
                    return Ok(new ApiErrorResponse(200, 
                        "Application created and sent successfully, " +
                        "but failed to send notifications to some devices."));
            }

            return Ok(app.ToDto());
        }

        // Delete application (recall unanswered application)
        [InvalidateCache("/applications")]
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

        // Get user's sent applications with spec
        [Cache(1000)]
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

        [InvalidateCache("/applications")]
        [HttpPost("/api/applications/document")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UploadApplicationDocument(IFormFile document)
        {
            // Check if an image was chosen
            if (document == null) 
                return BadRequest(new ApiErrorResponse(400, "No document found"));

            // Upload and get download Url
            var docUrl = await _uploadService.UploadFileAsync(
                document.OpenReadStream(),
                document.FileName,
                document.ContentType,
                _config["Firebase:ApplicationDocumentsUrl"]);

            if (docUrl == null) 
                return BadRequest(new ApiErrorResponse(400, "Failed to upload document."));

            return Ok(new ApiErrorResponse(200, "Document uploaded successfully.", docUrl));
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

        private static FCMessage CreateNewApplicationMessage(List<string> tokens, 
            Group group, int appId)
        {
            return new FCMessage
            {
                tokens = tokens,
                title = "New Application",
                body = $"{group.Name} has received a new application.",
                data = new Dictionary<string, string>
                {
                    { "type", "new_application" },
                    { "groupId", group.Id.ToString() },
                    { "applicationId", appId.ToString() }
                }
            };
        }

    }
}