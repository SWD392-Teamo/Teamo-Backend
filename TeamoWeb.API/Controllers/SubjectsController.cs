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
    }
}
