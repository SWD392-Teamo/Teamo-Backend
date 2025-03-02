using Teamo.Core.Entities;
using Teamo.Core.Specifications.Posts;

namespace Teamo.Core.Interfaces.Services
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetPostsAsync(PostSpecification spec);
        Task<Post> GetPostByIdAsync(int id); 
        Task<Post> CreatePost(Post post, int userId);
        Task<Post> UpdatePost(Post post, int userId);
        Task DeletePost(Post post, int userId);
    }
}
