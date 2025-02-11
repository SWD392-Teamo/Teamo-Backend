using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.StudentSkills
{
    public class StudentSkillForCompareSpecification : BaseSpecification<StudentSkill>
    {
        public StudentSkillForCompareSpecification(StudentSkillForCompareParams compareParams) 
            : base(x => x.StudentId == compareParams.StudentId )
        {
        }
    }
}