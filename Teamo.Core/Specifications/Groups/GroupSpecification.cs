
using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupSpecification : BaseSpecification<Group>
    {
        public GroupSpecification(GroupParams groupParams)
            : base(x => (string.IsNullOrEmpty(groupParams.Search)
            || x.GroupPositions.Any(gp => gp.Name.ToLower().Contains(groupParams.Search))) &&
            (!groupParams.SubjectId.HasValue || x.SubjectId == groupParams.SubjectId)
            )
        {
            AddInclude(x => x.GroupPositions);
            AddInclude(x => x.GroupMembers);
            AddInclude(x => x.Applications);
            AddInclude(x => x.Semester);
            AddInclude(x => x.CreatedByUser);
            AddInclude(x => x.Subject);
            AddInclude(x => x.Field);
            ApplyPaging(groupParams.PageSize * (groupParams.PageIndex - 1),
                groupParams.PageSize);
        }
        public GroupSpecification(int id)
            : base(x => x.Id == id)
        {
            AddInclude(x => x.GroupPositions);
            AddInclude(x => x.GroupMembers);
            AddInclude(x => x.Applications);
            AddInclude(x => x.Semester);
            AddInclude(x => x.CreatedByUser);
            AddInclude(x => x.Subject);
            AddInclude(x => x.Field);
        }
    }
}
