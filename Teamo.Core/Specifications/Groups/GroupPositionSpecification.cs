using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupPositionSpecification : BaseSpecification<GroupPosition>
    {
        public GroupPositionSpecification(int id) : base(x => x.Id == id && x.Status != GroupPositionStatus.Deleted)
        {
            AddInclude(gp => gp.Skills);
            AddInclude(gp => gp.GroupPositionSkills);
        }
        public GroupPositionSpecification(GroupPositionParams groupPositionParams)
            : base (x => (!groupPositionParams.GroupId.HasValue || groupPositionParams.GroupId == x.GroupId) &&
                (!groupPositionParams.PositionId.HasValue || groupPositionParams.PositionId == x.Id) &&
                x.Status != GroupPositionStatus.Deleted)
        {
            AddInclude(gp => gp.Skills);
            AddInclude(gp => gp.GroupPositionSkills);
            AddOrderByDescending(gp => gp.Status);
        }
    }
}