using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupMemberPositionSpecification : BaseSpecification<GroupMemberPosition>
    {
        public GroupMemberPositionSpecification(int groupMemberId)
            : base(x => x.GroupMemberId == groupMemberId)
        {

        }
    }
}
