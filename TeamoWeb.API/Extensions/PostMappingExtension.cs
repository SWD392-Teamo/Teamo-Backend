using System.Runtime.CompilerServices;
using Teamo.Core.Entities;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class PostMappingExtension
    {
        public static PostDto? ToDto(this Post? post)
        {
            if (post == null) return null;
            return new PostDto
            {
                Id = post.Id,
                GroupMemberId = post.GroupMemberId,
                GroupMemberName = $"{post.GroupMember.Student.FirstName} {post.GroupMember.Student.LastName}",
                GroupMemberImgUrl = post.GroupMember.Student.ImgUrl,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Privacy = post.Privacy,
                Status = post.Status,
            };
        }
    }
}
