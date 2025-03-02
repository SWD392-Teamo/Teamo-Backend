using Teamo.Core.Entities;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Posts;

namespace Teamo.Infrastructure.Services
{
    public class PostService : IPostService
    {
        public Task CreatePost(Post post)
        {
            throw new NotImplementedException();
        }

        public Task DeletePost(Post post)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetPostByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Post>> GetPostsAsync(PostSpecification spec)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePost(Post post)
        {
            throw new NotImplementedException();
        }
    }
}
