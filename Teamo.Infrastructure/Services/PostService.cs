using Teamo.Core.Entities;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications;
using Teamo.Core.Specifications.Groups;
using Teamo.Core.Specifications.Posts;

namespace Teamo.Infrastructure.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PostService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Post> CreatePost(Post post, int userId, int groupId)
        {
            var spec = new GroupMemberSpecification(new GroupMemberParams { GroupId = groupId, StudentId = userId });
            var groupMember = await _unitOfWork.Repository<GroupMember>().GetEntityWithSpec(spec);
            if (groupMember == null)
            {
                throw new UnauthorizedAccessException("You do not have permission to up a post in this group.");
            }

            post.GroupId = groupId;
            post.StudentId = userId;
            _unitOfWork.Repository<Post>().Add(post); 
            await _unitOfWork.Repository<Post>().SaveAllAsync();
            return await GetPostByIdAsync(post.Id); 
        }

        public async Task DeletePost(Post post, int userId)
        {
            if (post.StudentId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this post.");
            }
            post.Status = PostStatus.Deleted;
            _unitOfWork.Repository<Post>().Update(post);
            await _unitOfWork.Repository<Post>().SaveAllAsync();
        }

        public async Task<Post> GetPostByIdAsync(int id)
        {
            var spec = new PostSpecification(id);
            var post = await _unitOfWork.Repository<Post>().GetEntityWithSpec(spec);
            return post;
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(PostSpecification spec)
        {
           return await _unitOfWork.Repository<Post>().ListAsync(spec);
        }

        public async Task<(IEnumerable<Post>,int)> GetUserPosts(IEnumerable<int> groupIds, PagingParams pagingParams)
        {
            var spec = new PostSpecification(groupIds, pagingParams);
            var posts = await _unitOfWork.Repository<Post>().ListAsync(spec);

            var countSpec = new PostSpecification(groupIds);
            var total = (await _unitOfWork.Repository<Post>().ListAsync(countSpec)).Count();
            return (posts, total);
        }

        public async Task<Post> UpdatePost(Post post, int userId)
        {
            if (post.StudentId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to edit this post.");
            }
            post.UpdatedAt = DateTime.Now;
            post.Status = PostStatus.Edited;    
            _unitOfWork.Repository<Post>().Update(post);
            await _unitOfWork.Repository<Post>().SaveAllAsync();
            return await GetPostByIdAsync(post.Id);
        }
    }
}
