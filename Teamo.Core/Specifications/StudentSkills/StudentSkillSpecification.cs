using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.StudentSkills
{
    public class StudentSkillSpecification : BaseSpecification<StudentSkill>
    {
        public StudentSkillSpecification(int skillId, int studentId)
            : base(x => x.SkillId == skillId && x.StudentId == studentId)
        {
            AddInclude(x => x.Skill);
            AddInclude(x => x.Student);
        }
    }
}