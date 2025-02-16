using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
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

        //Get subjects with spec
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

        //Get subject by id
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

            var duplicateCode = await _subjectService.CheckDuplicateCodeSubject(subjectDto.Code);
            if(!duplicateCode) return BadRequest(new ApiErrorResponse(400, "Subject code must be unique."));
            
            var subject = subjectDto.ToEntity();

            var result = await _subjectService.CreateSubjectAsync(subject);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to create new subject."));
            else return Ok(new ApiErrorResponse(200, "Created new subject successfully."));
        }

        //Update subject, update name or description only
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateSubject(int id, [FromBody] SubjectDto subjectDto)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if(subject == null) return NotFound(new ApiErrorResponse(404, "Subject not found."));

            subject = subjectDto.ToEntity(subject);

            var result = await _subjectService.UpdateSubjectAsync(subject);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to update subject."));
            else return Ok(new ApiErrorResponse(200, "Updated subject successfully."));
        }

        //Delete subject, change subject status to inactive
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteSubject(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if(subject == null) return BadRequest(new ApiErrorResponse(400, "Subject not found."));
            if(subject.Status == SubjectStatus.Inactive)
                return BadRequest(new ApiErrorResponse(400, "Subject is already inactive."));

            var result = await _subjectService.DeleteSubjectAsync(subject);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to delete subject"));
            return Ok(new ApiErrorResponse(200, "Deleted subject successfully."));
        }

    }
}
