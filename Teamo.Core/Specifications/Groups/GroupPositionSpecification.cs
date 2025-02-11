using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupPositionSpecification : BaseSpecification<GroupPosition>
    {
        public GroupPositionSpecification(int id) : base(x => x.Id == id)
        {
        }
        public GroupPositionSpecification(GroupPositionParams groupPositionParams)
            : base (x => (!groupPositionParams.GroupId.HasValue || groupPositionParams.GroupId == x.GroupId) &&
                (!groupPositionParams.PositionId.HasValue || groupPositionParams.PositionId == x.Id))
        {
            
        }
    }
}