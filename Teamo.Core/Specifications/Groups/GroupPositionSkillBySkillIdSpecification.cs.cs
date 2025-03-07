using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupPositionSkillBySkillIdSpecification : BaseSpecification<GroupPositionSkill>
    {
        public GroupPositionSkillBySkillIdSpecification(int skillId)
            : base(x => x.SkillId == skillId)
        {
            
        }
    }
}