﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Specifications.Semesters;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{
    public class SemestersController : BaseApiController
    {
        private readonly IGenericRepository<Semester> _semesterRepo;
        public SemestersController(IGenericRepository<Semester> semesterRepo)
        {
            _semesterRepo = semesterRepo;
        }

        [Cache(1000)]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<SemesterDto>>> GetSemesters([FromQuery] SemesterParams semesterParams)
        {
            var spec = new SemesterSpecification(semesterParams);
            return await CreatePagedResult(_semesterRepo, spec, semesterParams.PageIndex,
                semesterParams.PageSize, s => s.ToDto());
        }

        [Cache(1000)]
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SemesterDto>> GetSemesterById(int id)
        {
            var semester = await _semesterRepo.GetByIdAsync(id);
            if (semester == null)
            {
                return NotFound(new ApiErrorResponse(404, "Semester not found"));
            }
            return Ok(semester.ToDto());
        }

        [InvalidateCache("/semesters")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SemesterDto>> CreateSemester(SemesterToUpsertDto semesterDto)
        {
            var semester = semesterDto.ToEntity();
            _semesterRepo.Add(semester);
            await _semesterRepo.SaveAllAsync();
            return Ok(semester.ToDto());
        }

        [InvalidateCache("/semesters")]
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SemesterDto>> UpdateSemester(int id, SemesterToUpsertDto semesterDto)
        {

            var semester = await _semesterRepo.GetByIdAsync(id);
            if(semester == null)
                return NotFound();

            semester = semesterDto.ToEntity(semester);
            _semesterRepo.Update(semester);
            await _semesterRepo.SaveAllAsync();
            return Ok(semester.ToDto());            
        }
    }
}
