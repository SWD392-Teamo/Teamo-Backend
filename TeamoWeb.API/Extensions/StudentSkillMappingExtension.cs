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
                SkillName = studentSkill.Skill.Name,
                SkillType = studentSkill.Skill.Type,
                SkillLevel = studentSkill.Level.ToString()
            };
        }

        public static StudentSkill ToEntity(this StudentSkillToUpsertDto studentSkillDto, StudentSkill? studentSkill = null)
        {
            Enum.TryParse(studentSkillDto.Level, out StudentSkillLevel level);

            //Add skill to user profile
            if(studentSkill == null)
            {
                return new StudentSkill{
                    SkillId = studentSkillDto.SkillId,
                    StudentId = studentSkillDto.StudentId,
                    Level = level
                };
            }

            //Update level of skill in user profile
            studentSkill.Level = level;

            return studentSkill;
        }
    }
}