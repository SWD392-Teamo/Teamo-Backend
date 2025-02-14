using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
    public class Post : BaseEntity
    {
        public required int GroupMemberId { get; set; }
        public GroupMember GroupMember { get; set; }
        public string Content { get; set; }
        public PostStatus Status { get; set; }
        public PostPrivacy Privacy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}