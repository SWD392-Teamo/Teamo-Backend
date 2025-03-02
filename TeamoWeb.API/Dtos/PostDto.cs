using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class PostDto
    {
        public int Id { get; set; } 
        public int GroupMemberId { get; set; }
        public string GroupMemberName { get; set; } = string.Empty;
        public string? GroupMemberImgUrl { get; set; }
        public string Content { get; set; } = string.Empty;
        public PostStatus Status { get; set; }
        public PostPrivacy Privacy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}

