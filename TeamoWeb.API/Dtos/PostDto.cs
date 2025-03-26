using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class PostDto
    {
        public int Id { get; set; } 
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string GroupMemberName { get; set; } = string.Empty;
        public string? GroupMemberImgUrl { get; set; }
        public string Content { get; set; } = string.Empty;
        public PostStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public string? DocumentUrl { get; set; }
    }
}

