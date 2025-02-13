using Microsoft.AspNetCore.Authorization;
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
    }
}