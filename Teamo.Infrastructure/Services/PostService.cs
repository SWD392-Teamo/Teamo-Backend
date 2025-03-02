using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
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
        public Task CreatePost(Post post)
        {
            throw new NotImplementedException();
        }

        public Task DeletePost(Post post)
        {
            throw new NotImplementedException();
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

        public Task UpdatePost(Post post)
        {
            throw new NotImplementedException();
        }
    }
}
