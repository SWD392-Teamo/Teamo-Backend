using Microsoft.EntityFrameworkCore;
using Teamo.Core.Constants;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Core.Specifications.Posts
{
    public class PostSpecification : BaseSpecification<Post>
    {
        public PostSpecification(PostParams postParams, bool? isApplyPaging = true)
            : base(x => (!postParams.GroupId.HasValue || postParams.GroupId.Value == x.GroupId) &&
            x.Status != PostStatus.Deleted)
        {
            {
                AddInclude(p => p.Student);
                if (isApplyPaging == true)
                {
                    ApplyPaging(postParams.PageSize * (postParams.PageIndex - 1),
                                postParams.PageSize);
                }
                if(!string.IsNullOrEmpty(postParams.Sort))
                {
                    switch (postParams.Sort)
                    {
                        case SortOptions.DateAsc:
                            AddOrderBy(p => p.CreatedAt);
                            break;
                        case SortOptions.DateDesc:
                            AddOrderByDescending(p => p.CreatedAt);
                            break;
                        default:
                            AddOrderByDescending(p => p.CreatedAt);
                            break;
                    }
                }
            }
        }

        public PostSpecification(int id)
            : base(p => p.Id == id && p.Status != PostStatus.Deleted)
        {
            {
                AddInclude(p => p.Student);
            }
        }
    } 
}
