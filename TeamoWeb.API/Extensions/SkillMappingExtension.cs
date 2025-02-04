using Teamo.Core.Entities;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class SkillMappingExtension
    {
        public static SkillDto? ToDto(this Skill? skill)
        {
            if(skill == null) return null;
            return new SkillDto
            {
                Id = skill.Id,
                Name = skill.Name,
                Type = skill.Type
            };
        }
    }
}