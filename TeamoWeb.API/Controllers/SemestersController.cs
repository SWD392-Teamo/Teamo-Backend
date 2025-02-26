﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces;
using Teamo.Core.Specifications.Majors;
using Teamo.Core.Specifications.Semesters;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;

namespace TeamoWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemestersController : BaseApiController
    {
        private readonly IGenericRepository<Semester> _semesterRepo;
        public SemestersController(IGenericRepository<Semester> semesterRepo)
        {
            _semesterRepo = semesterRepo;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<Semester>>> GetSemesters([FromQuery] SemesterParams semesterParams)
        {
            var spec = new SemesterSpecification(semesterParams);
            return await CreatePagedResult(_semesterRepo, spec, semesterParams.PageIndex,
                semesterParams.PageSize, s => s.ToDto());
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Semester>> GetSemesterById(int id)
        {
            var semester = await _semesterRepo.GetByIdAsync(id);
            if (semester == null)
            {
                return NotFound();
            }
            return Ok(semester.ToDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Semester>> CreateSemester(SemesterToUpsertDto semesterDto)
        {
            var semester = semesterDto.ToEntity();
            _semesterRepo.Add(semester);
            await _semesterRepo.SaveAllAsync();
            return Ok(semester.ToDto());
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Semester>> UpdateSemester(int id, SemesterToUpsertDto semesterDto)
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
