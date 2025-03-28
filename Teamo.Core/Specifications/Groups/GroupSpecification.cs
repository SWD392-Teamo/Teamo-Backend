﻿
using Microsoft.EntityFrameworkCore;
using Teamo.Core.Constants;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupSpecification : BaseSpecification<Group>
    {
        public GroupSpecification(GroupParams groupParams, bool applyPaging = true, bool isAdmin = false)
            : base(x => (string.IsNullOrEmpty(groupParams.Search)
            || x.GroupPositions.Any(gp => gp.Name.ToLower().Contains(groupParams.Search))
            || x.Name.ToLower().Contains(groupParams.Search)
            || x.Title.ToLower().Contains(groupParams.Search)) &&
            (!groupParams.SubjectId.HasValue || x.SubjectId == groupParams.SubjectId) &&
            (!groupParams.Status.HasValue ? x.Status != GroupStatus.Deleted : groupParams.Status == x.Status) &&
            (!groupParams.SemesterId.HasValue || groupParams.SemesterId == x.SemesterId) &&
            (!groupParams.FieldId.HasValue || groupParams.FieldId == x.FieldId) &&
            (!groupParams.StudentId.HasValue || x.GroupMembers.Any(gm => gm.StudentId == groupParams.StudentId)) &&
            (isAdmin || (x.Status != GroupStatus.Banned && x.Status != GroupStatus.Deleted)))         
        {
            AddThenInclude(q => q.Include(x => x.GroupPositions).ThenInclude(a => a.Skills));
            AddInclude(x => x.CreatedByUser);
            AddThenInclude(q => q.Include(x => x.Applications).ThenInclude(a => a.Student));
            AddInclude(x => x.Semester);
            AddInclude(x => x.Subject);
            AddInclude(x => x.Field);
            AddThenInclude(q => q.Include(x => x.GroupMembers).ThenInclude(u => u.Student));
            AddThenInclude(q => q.Include(x => x.GroupMembers).ThenInclude(u => u.GroupPositions));
            if(applyPaging)
            {
                ApplyPaging(groupParams.PageSize * (groupParams.PageIndex - 1),
                groupParams.PageSize);
            }

            if (!string.IsNullOrEmpty(groupParams.Sort))
            {
                switch (groupParams.Sort)
                {
                    case SortOptions.DateAsc:
                        AddOrderBy(x => x.CreatedAt);
                        break;
                    case SortOptions.DateDesc:
                        AddOrderByDescending(x => x.CreatedAt);
                        break;
                    default:
                        AddOrderByDescending(x => x.CreatedAt);
                        break;
                }
            }

        }
        public GroupSpecification(int id)
            : base(x => x.Id == id)
        {
            AddThenInclude(q => q.Include(x => x.GroupPositions).ThenInclude(a => a.Skills));
            AddThenInclude(q => q.Include(x => x.Applications).ThenInclude(a => a.Student));
            AddInclude(x => x.Semester);
            AddInclude(x => x.CreatedByUser);
            AddInclude(x => x.Subject);
            AddInclude(x => x.Field);
            AddThenInclude(q => q.Include(x => x.GroupMembers).ThenInclude(u => u.Student));
            AddThenInclude(q => q.Include(x => x.GroupMembers).ThenInclude(u => u.GroupPositions));
        }

        //Group specification for status update based on semester
        public GroupSpecification()
        {
            AddInclude(x => x.Semester);
        }
    }
}
