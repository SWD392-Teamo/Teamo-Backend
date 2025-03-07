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
        
        public static Skill ToEntity(this SkillDto skillDto, Skill? skill = null)
        {
            //Add skill
            if(skill == null)
            {
                return new Skill{
                    Name = skillDto.Name,
                    Type = skillDto.Type
                };
            }

            //Update skill
            skill.Name = string.IsNullOrEmpty(skillDto.Name) ? skill.Name : skillDto.Name;
            skill.Type = string.IsNullOrEmpty(skillDto.Type) ? skill.Type : skillDto.Type;

            return skill;
        }
    }
}