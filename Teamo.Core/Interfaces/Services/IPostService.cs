using Teamo.Core.Entities;
using Teamo.Core.Specifications.Posts;

namespace Teamo.Core.Interfaces.Services
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetPostsAsync(PostSpecification spec);
        Task<Post> GetPostByIdAsync(int id); 
        Task<Post> CreatePost(Post post);
        Task UpdatePost(Post post);
        Task DeletePost(Post post);
    }
}
