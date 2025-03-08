using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Specifications.Skills;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{
    public class SkillsController : BaseApiController
    {
        private readonly ISkillService _skillService;

        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [Cache(1000)]
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SkillDto>> GetSkillById(int id)
        {
            var skill = await _skillService.GetSkillByIdAsync(id);
            
            if(skill == null) return NotFound(new ApiErrorResponse(404, "Skill not found."));
            return Ok(skill.ToDto());
        }

        [Cache(1000)]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<SkillDto>>> GetSkills([FromQuery] SkillParams skillParams)
        {
            var skills = await _skillService.GetSkillsWithSpecAsync(skillParams);
            var skillsToDtos = skills.Select(s => s.ToDto()).ToList();
            return Ok(skillsToDtos);
        }

        [InvalidateCache("/skills")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SkillDto>> CreateSkill([FromBody] SkillDto skill)
        {
            if(skill == null || string.IsNullOrEmpty(skill.Name) || string.IsNullOrEmpty(skill.Type))
                return BadRequest(new ApiErrorResponse(400, "Please input all required fields."));

            //Check for duplicates
            var check = await _skillService.CheckDuplicateSkillAsync(skill.Name);
            if(!check) return BadRequest(new ApiErrorResponse(400, "Already exists skill with this name."));
            
            var newSkill = await _skillService.CreateSkillAsync(skill.ToEntity());
            if(newSkill == null) return BadRequest(new ApiErrorResponse(400, "Failed to create skill."));
            return Ok(newSkill.ToDto());
        }

        [InvalidateCache("/skills")]
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SkillDto>> UpdateSkill(int id, [FromBody] SkillDto skillDto)
        {
            var skill = await _skillService.GetSkillByIdAsync(id);
            if(skill == null) return NotFound(new ApiErrorResponse(404, "Skill not found."));

            if(!string.IsNullOrEmpty(skillDto.Name))
            {
                var check = await _skillService.CheckDuplicateSkillAsync(skillDto.Name);
                if(!check) return BadRequest(new ApiErrorResponse(400, "Already exists skill with this name."));
            }

            skill = skillDto.ToEntity(skill);
            var updatedSkill = await _skillService.UpdateSkillAsync(skill);

            if(updatedSkill == null) return BadRequest(new ApiErrorResponse(400, "Failed to update skill."));
            else return Ok(updatedSkill.ToDto());
        }

        [InvalidateCache("/skills")]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteSkill(int id)
        {
            var skill = await _skillService.GetSkillByIdAsync(id);
            if(skill == null) return NotFound(new ApiErrorResponse(404, "Skill not found"));
            
            var result = await _skillService.DeleteSkillAsync(skill);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to delete skill."));
            return Ok(new ApiErrorResponse(200, "Deleted skill successfully.")); 
        }
    }
}