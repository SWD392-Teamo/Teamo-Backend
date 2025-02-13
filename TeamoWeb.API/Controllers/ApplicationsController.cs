using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Applications;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{
    public class ApplicationsController : BaseApiController
    {
        private readonly IApplicationService _appService;

        public ApplicationsController(IApplicationService appService)
        {
            _appService = appService;
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

        //Get user's sent applications with spec
        [HttpGet("my-apps")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IReadOnlyList<ApplicationDto>>> GetSentApplications([FromQuery] ApplicationParams appParams)
        {
            var appSpec = new ApplicationSpecification(appParams);
            var apps = await _appService.GetSentApplicationsAsync(appSpec);
            var count = await _appService.CountAsync(appSpec);
            var appsToDto = apps.Select(a => a.ToDto()).ToList();
            var pagination = new Pagination<ApplicationDto>(appParams.PageIndex,appParams.PageSize,count,appsToDto);
            return Ok(pagination);
        }
        
        //Get group's applications with spec
        [HttpGet("group-apps")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IReadOnlyList<ApplicationDto>>> GetGroupApplications([FromQuery] ApplicationParams appParams)
        {
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
        public async Task<ActionResult> ReviewApplication(int id, [FromBody] ApplicationDto appReviewDto)
        {
            var appReview = await _appService.GetApplicationByIdAsync(id);
            if(appReview == null || appReview.Status != ApplicationStatus.Requested) 
                return BadRequest(new ApiErrorResponse(400, "Unable to review this application."));
        
            //Check if current user is the leader of corresponding group
            var groupLeaderEmail = await _appService.GetGroupLeaderEmailAsync(appReview.GroupId);
            if(!User.GetEmail().Equals(groupLeaderEmail)) 
                return BadRequest(new ApiErrorResponse(400, "Only group leaders can review applications."));

            var result = await _appService.ReviewApplicationAsync(appReview, appReviewDto.Status);
            if(result) return Ok(new ApiErrorResponse(200, "Application reviewed successfully."));
            else return BadRequest(new ApiErrorResponse(200, "Failed to review application."));
        }

        //Create and send an application
        [HttpPost("new-app")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApplicationDto>> CreateNewApplication([FromBody] ApplicationDto app)     
        {
            //Check for validity to apply
            var isValid = await _appService.CheckValidToApply(app.GroupId,app.StudentId,app.GroupPositionId);
            if(!isValid) return BadRequest(new ApiErrorResponse(400, "Not applicable to create applications."));

            var newApp = new Application(){
                GroupId = app.GroupId,
                StudentId = app.StudentId,
                RequestTime = app.RequestTime,
                RequestContent = app.RequestContent,
                GroupPositionId = app.GroupPositionId,
                Status = ApplicationStatus.Requested
            };

            var result = await _appService.CreateNewApplicationAsync(newApp);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to create and send application."));
            return Ok(new ApiErrorResponse(200, "Application sent successfully."));
        }
    }
}