using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Specifications.Skills;
using TeamoWeb.API.Errors;

namespace TeamoWeb.API.Controllers
{
    public class SkillsController : BaseApiController
    {
        private readonly ISkillService _skillService;

        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Skill>> GetSkillById(int id)
        {
            var skill = await _skillService.GetSkillByIdAsync(id);
            
            if(skill == null) return NotFound(new ApiErrorResponse(404, "Skill not found."));
            return Ok(skill);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<Skill>>> GetSkills([FromQuery] SkillParams skillParams)
        {
            var skills = await _skillService.GetSkillsWithSpecAsync(skillParams);
            return Ok(skills);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateSkill([FromBody] Skill skill)
        {
            if(skill == null || string.IsNullOrEmpty(skill.Name) || string.IsNullOrEmpty(skill.Type))
                return BadRequest(new ApiErrorResponse(400, "Please input all required fields."));

            //Check for duplicates
            var check = await _skillService.CheckDuplicateSkillAsync(skill.Name);
            if(!check) return BadRequest(new ApiErrorResponse(400, "Already exists skill with this name."));
            
            var result = await _skillService.CreateSkillAsync(skill);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to create skill."));
            return Ok(new ApiErrorResponse(200, "Skill created successfully."));
        }
    }
}