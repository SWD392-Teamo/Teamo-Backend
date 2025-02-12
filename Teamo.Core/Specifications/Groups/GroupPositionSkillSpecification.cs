
using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupPositionSkillSpecification : BaseSpecification<GroupPositionSkill>
    {
        public GroupPositionSkillSpecification(int groupPositionId)
            : base (x => x.GroupPositionId == groupPositionId)
        {
            
        }
    }
}
