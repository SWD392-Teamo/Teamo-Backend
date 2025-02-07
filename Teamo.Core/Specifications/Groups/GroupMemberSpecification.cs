using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupMemberSpecification : BaseSpecification<GroupMember>
    {
        public GroupMemberSpecification(int groupId, GroupMemberRole role)
            : base(x => x.GroupId == groupId && x.Role == role)
        {
        }

        public GroupMemberSpecification(int groupId, int studentId)
            : base(x => x.GroupId == groupId && x.StudentId == studentId)
            {
            }
    }
}