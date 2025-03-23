using System.Runtime.CompilerServices;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
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
                GroupId = post.GroupId,
                StudentId = post.StudentId,
                GroupMemberName = $"{post.Student.FirstName} {post.Student.LastName}",
                GroupMemberImgUrl = post.Student.ImgUrl,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Status = post.Status,
                DocumentUrl = post.DocumentUrl,
                GroupName = post.Group.Name
            };
        }

        public static Post ToEntity(this PostToUpsertDto postToUpsertDto, Post? post = null)
        {
            if(post == null)
            {
                if (string.IsNullOrEmpty(postToUpsertDto.Content))
                    throw new ArgumentException("Content cannot be empty when creating a new post.");
                return new Post
                {
                    Content = postToUpsertDto.Content,
                };
            }

            post.Content = string.IsNullOrEmpty(postToUpsertDto.Content) ? post.Content : postToUpsertDto.Content;
            post.UpdatedAt = DateTime.Now;
            return post;
        }
    }
}
