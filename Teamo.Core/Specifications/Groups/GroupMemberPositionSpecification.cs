using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupMemberPositionSpecification : BaseSpecification<GroupMemberPosition>
    {
        public GroupMemberPositionSpecification(int? groupMemberId = null, int? groupPositionId = null)
            : base(x => (!groupMemberId.HasValue || x.GroupMemberId == groupMemberId) &&
            (!groupPositionId.HasValue || x.GroupPositionId == groupPositionId))
        {
        }
    }
}
