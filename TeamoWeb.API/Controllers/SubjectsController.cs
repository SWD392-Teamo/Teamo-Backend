using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Specifications.Subjects;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Extensions;

namespace TeamoWeb.API.Controllers
{
    public class SubjectsController : BaseApiController
    {
        private readonly IGenericRepository<Subject> _subjectsRepository;
        public SubjectsController(IGenericRepository<Subject> subjectsRepository)
        {
            _subjectsRepository = subjectsRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<SubjectDto>>> GetSubjects([FromQuery] SubjectParams specParams)
        {
            var spec = new SubjectSpecification(specParams);
            var subjects = await CreatePagedResult(_subjectsRepository, spec, specParams.PageIndex, 
                specParams.PageSize, s => s.ToDto());
            return Ok(subjects);
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SubjectDto?>> GetSubjectById(int id)
        {
            var spec = new SubjectSpecification(id);
            var subject = await _subjectsRepository.GetEntityWithSpec(spec);
            if (subject == null) 
                return NotFound();
            return subject.ToDto();
        }
    }
}
