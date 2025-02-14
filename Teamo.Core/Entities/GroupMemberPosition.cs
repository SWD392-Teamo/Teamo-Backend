namespace Teamo.Core.Entities
{
    public class GroupMemberPosition : BaseEntity
    {
        public required int GroupPositionId { get; set; }
        public GroupPosition GroupPosition { get; set; }
        public required int GroupMemberId { get; set; }
        public GroupMember GroupMember { get; set; }
    }
}