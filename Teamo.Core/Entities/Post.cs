using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
    public class Post : BaseEntity
    {
        public int GroupMemberId { get; set; }
        public GroupMember GroupMember { get; set; }
        public string Content { get; set; }
        public PostStatus Status { get; set; } = PostStatus.Posted;
        public PostPrivacy Privacy { get; set; } = PostPrivacy.Public;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}