﻿
using Microsoft.EntityFrameworkCore;
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
            AddInclude(x => x.CreatedByUser);
            AddThenInclude(q => q.Include(x => x.Applications).ThenInclude(a => a.SrcStudent));
            AddInclude(x => x.Semester);
            AddInclude(x => x.Subject);
            AddInclude(x => x.Field);
            AddThenInclude(q => q.Include(x => x.GroupMembers).ThenInclude(u => u.Student));
            ApplyPaging(groupParams.PageSize * (groupParams.PageIndex - 1),
                groupParams.PageSize);
        }
        public GroupSpecification(int id)
            : base(x => x.Id == id)
        {
            AddInclude(x => x.GroupPositions);
            AddThenInclude(q => q.Include(x => x.Applications).ThenInclude(a => a.SrcStudent));
            AddInclude(x => x.Semester);
            AddInclude(x => x.CreatedByUser);
            AddInclude(x => x.Subject);
            AddInclude(x => x.Field);
            AddThenInclude(q => q.Include(x => x.GroupMembers).ThenInclude(u => u.Student));
        }
    }
}
