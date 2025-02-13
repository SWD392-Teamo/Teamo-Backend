using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Majors;
using Teamo.Core.Specifications.Subjects;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{
    public class SubjectsController : BaseApiController
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<SubjectDto>>> GetSubjects([FromQuery] SubjectParams subjectParams)
        {
            var subjects = await _subjectService.GetSubjectsAsync(subjectParams);
            var count = await _subjectService.CountSubjectsAsync(subjectParams);
            var subjectsToDto = subjects.Select(s => s.ToDto()).ToList();
            var pagination = new Pagination<SubjectDto>(subjectParams.PageIndex,subjectParams.PageSize,count,subjectsToDto);

            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SubjectDto?>> GetSubjectById(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);

            if (subject == null) 
                return NotFound(new ApiErrorResponse(404, "Subject not found"));

            return subject.ToDto();
        }


        //Create a new subject
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateNewSubject([FromBody] SubjectDto subjectDto)
        {
            if(string.IsNullOrEmpty(subjectDto.Name) || string.IsNullOrEmpty(subjectDto.Code))
                return BadRequest(new ApiErrorResponse(400, "Please input all required fields."));

            var subject = subjectDto.ToEntity();

            var result = await _subjectService.CreateSubjectAsync(subject);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to create new subject."));
            else return Ok(new ApiErrorResponse(200, "Created new subject successfully."));
        }

        //Update subject, update name or description only
        [HttpPatch]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateSubject([FromBody] SubjectDto subjectDto)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(subjectDto.Id);
            if(subject == null) return NotFound(new ApiErrorResponse(404, "Subject not found."));

            subject = subjectDto.ToEntity(subject);

            var result = await _subjectService.UpdateSubjectAsync(subject);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to update subject."));
            else return Ok(new ApiErrorResponse(200, "Updated subject successfully."));
        }
    }
}
