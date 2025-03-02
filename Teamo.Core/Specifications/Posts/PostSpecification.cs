using Microsoft.EntityFrameworkCore;
using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Posts
{
    public class PostSpecification : BaseSpecification<Post>
    {
        public PostSpecification(PostParams postParams)
            : base(x => (!postParams.GroupId.HasValue || postParams.GroupId.Value == x.GroupMember.GroupId))
        {
            {
                AddThenInclude(p => p.Include(p => p.GroupMember).ThenInclude(gm => gm.Student));
            }
        }
    } 
}
