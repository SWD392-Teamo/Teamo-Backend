using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
    public class Post : BaseEntity
    {
        public int StudentId { get; set; }
        public User Student { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public string Content { get; set; }
        public PostStatus Status { get; set; } = PostStatus.Posted;
        public PostPrivacy Privacy { get; set; } = PostPrivacy.Public;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}