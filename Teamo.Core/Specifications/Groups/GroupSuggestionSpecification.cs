
using Microsoft.EntityFrameworkCore;
using Teamo.Core.Constants;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupSuggestionSpecification : BaseSpecification<Group>
    {
        public GroupSuggestionSpecification(int? semesterId)
            : base(x => 
            (x.Status == GroupStatus.Recruiting) &&
            (!semesterId.HasValue || semesterId == x.SemesterId))          
        {
            AddInclude(x => x.Field);
            AddInclude(x => x.Subject);
            AddThenInclude(q => q.Include(x => x.GroupPositions).ThenInclude(a => a.Skills));
        }
    }
}
