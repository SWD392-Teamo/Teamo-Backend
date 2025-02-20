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
        private readonly IUserService _userService;

        public ApplicationsController(IApplicationService appService, IUserService userService)
        {
            _appService = appService;
            _userService = userService;
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
        [HttpGet]
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

    }
}