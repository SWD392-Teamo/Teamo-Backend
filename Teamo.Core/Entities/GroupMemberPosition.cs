namespace Teamo.Core.Entities
{
    public class GroupMemberPosition : BaseEntity
    {
        public int GroupPositionId { get; set; }
        public GroupPosition GroupPosition { get; set; }
        public int GroupMemberId { get; set; }
        public GroupMember GroupMember { get; set; }
    }
}