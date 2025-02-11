using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupMemberSpecification : BaseSpecification<GroupMember>
    {
        public GroupMemberSpecification(GroupMemberParams groupMemberParams)
            : base(x => (!groupMemberParams.GroupId.HasValue || groupMemberParams.GroupId == x.GroupId) && 
                        (!groupMemberParams.Role.HasValue || groupMemberParams.Role == x.Role) &&
                        (!groupMemberParams.Studentd.HasValue || groupMemberParams.Studentd == x.StudentId))
        {
        }
    }
}