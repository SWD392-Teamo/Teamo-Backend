using Teamo.Core.Entities;
using Teamo.Core.Enums;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class StudentSkillMappingExtension
    {
        public static StudentSkillDto? ToDto(this StudentSkill? studentSkill)
        {
            if(studentSkill == null) return null;
            return new StudentSkillDto
            {
                SkillId = studentSkill.SkillId,
                SkillName = studentSkill.Skill.Name,
                SkillType = studentSkill.Skill.Type,
                SkillLevel = studentSkill.Level.ToString()
            };
        }
    }
}