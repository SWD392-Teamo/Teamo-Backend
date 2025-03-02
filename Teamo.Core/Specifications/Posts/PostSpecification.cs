using Microsoft.EntityFrameworkCore;
using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Posts
{
    public class PostSpecification : BaseSpecification<Post>
    {
        public PostSpecification(PostParams postParams, bool? isApplyPaging = true)
            : base(x => (!postParams.GroupId.HasValue || postParams.GroupId.Value == x.GroupMember.GroupId))
        {
            {
                AddThenInclude(p => p.Include(p => p.GroupMember).ThenInclude(gm => gm.Student));
                if (isApplyPaging == true)
                {
                    ApplyPaging(postParams.PageSize * (postParams.PageIndex - 1),
                                postParams.PageSize);
                }
            }
        }

        public PostSpecification(int id)
            : base(p => p.Id == id)
        {
            {
                AddThenInclude(p => p.Include(p => p.GroupMember).ThenInclude(gm => gm.Student));
            }
        }
    } 
}
