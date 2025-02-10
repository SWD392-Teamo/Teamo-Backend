using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Specifications.Majors;
using Teamo.Infrastructure.Services;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Extensions;

namespace TeamoWeb.API.Controllers
{
    public class MajorsController : BaseApiController
    {
        private readonly IGenericRepository<Major> _majorRepo;

        public MajorsController(IGenericRepository<Major> majorRepo)
        {
            _majorRepo = majorRepo;
        }

        //Get list of majors with spec
        [HttpGet]
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult<IReadOnlyList<MajorDto>>> GetMajors([FromQuery] MajorSpecParams majorSpecParams)
        {
            var majorSpec = new MajorSpecification(majorSpecParams);
            return await CreatePagedResult(_majorRepo, majorSpec, majorSpecParams.PageIndex,
                majorSpecParams.PageSize, m => m.ToDto());
        }

        //Get major by Id
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult<MajorDto?>> GetMajorById(int id)
        {
            var majorSpec = new MajorSpecification(id);
            var major = await _majorRepo.GetEntityWithSpec(majorSpec);
            if (major == null) return NotFound();
            return Ok(major.ToDto());
        }
    }
}