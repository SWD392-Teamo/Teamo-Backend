using Microsoft.EntityFrameworkCore;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupMemberSpecification : BaseSpecification<GroupMember>
    {
        public GroupMemberSpecification(GroupMemberParams groupMemberParams)
            : base(x => (!groupMemberParams.GroupId.HasValue || groupMemberParams.GroupId == x.GroupId) && 
                        (!groupMemberParams.Role.HasValue || groupMemberParams.Role == x.Role) &&
                        (!groupMemberParams.StudentId.HasValue || groupMemberParams.StudentId == x.StudentId))
        {
            AddInclude(gm => gm.Student);           
            AddInclude(gm => gm.GroupMemberPositions);
            AddInclude(gm => gm.GroupPositions);
            ApplyPaging(groupMemberParams.PageSize * (groupMemberParams.PageIndex - 1),
                groupMemberParams.PageSize);
        }
        public GroupMemberSpecification(int id)
            :base(x => x.Id == id)
        {
            AddInclude(gm => gm.GroupMemberPositions);
            AddInclude(gm => gm.Student);
        }
    }
}