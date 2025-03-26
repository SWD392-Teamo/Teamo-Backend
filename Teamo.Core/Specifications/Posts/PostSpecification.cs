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
                AddInclude(p => p.Group);
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
                AddOrderByDescending(p => p.CreatedAt);
            }
        }

        public PostSpecification(int id)
            : base(p => p.Id == id && p.Status != PostStatus.Deleted)
        {
            AddInclude(p => p.Student);
            AddInclude(p => p.Group);
        }

        public PostSpecification(IEnumerable<int> groupIds, PagingParams? pagingParams = null)
            : base(x => groupIds.Contains(x.GroupId))
        {
            AddInclude(p => p.Student);
            AddInclude(p => p.Group);
            AddOrderByDescending(p => p.CreatedAt);
            if(pagingParams != null)
            {
                ApplyPaging(pagingParams.PageSize * (pagingParams.PageIndex - 1),
                                pagingParams.PageSize);
            }
        }
    } 
}
