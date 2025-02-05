using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces;
using Teamo.Core.Specifications.Applications;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Extensions;

namespace TeamoWeb.API.Controllers
{
    public class ApplicationsController : BaseApiController
    {
        private readonly IGenericRepository<Application> _appRepo;

        public ApplicationsController(IGenericRepository<Application> appRepo)
        {
            _appRepo = appRepo;
        }

        //Get application by id
        [HttpGet("{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ApplicationDto?>> GetApplicationById(int id)
        {
            var appSpec = new ApplicationSpecification(id);
            var app = await _appRepo.GetEntityWithSpec(appSpec);
            if(app == null) return NotFound();
            return Ok(app.ToDto());
        }

        //Get user's sent applications with spec
        [HttpGet("my-apps")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IReadOnlyList<ApplicationDto>>> GetSentApplications([FromQuery] ApplicationParams appParams)
        {
            var appSpec = new ApplicationGroupSpecification(appParams);
            return await CreatePagedResult(_appRepo, appSpec, 
                appParams.PageIndex, appParams.PageSize, a => a.ToDto());
        }
        
        //Get group's applications with spec
        [HttpGet("group-apps")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IReadOnlyList<ApplicationDto>>> GetGroupApplications([FromQuery] ApplicationParams appParams)
        {
            var appSpec = new ApplicationGroupSpecification(appParams);
            return await CreatePagedResult(_appRepo, appSpec, 
                appParams.PageIndex, appParams.PageSize, a => a.ToDto());
        }

        //Approve or reject application
        [HttpPatch]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> ReviewApplication([FromBody] ApplicationDto appReviewDto)
        {
            var appSpec = new ApplicationSpecification(appReviewDto.Id);
            var appReview = await _appRepo.GetEntityWithSpec(appSpec);

            //Check if current user is the leader of corresponding group
            if(!User.GetEmail().Equals(appReview.DestStudent.Email)) return BadRequest();

            Enum.TryParse(appReviewDto.Status, out ApplicationStatus appStatus);
            appReview.Status = appStatus;
            _appRepo.Update(appReview);
            var result = await _appRepo.SaveAllAsync();
            if(result) return Ok();
            else return BadRequest();
        }
    }
}