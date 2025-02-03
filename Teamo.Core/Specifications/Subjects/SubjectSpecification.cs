using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Subjects
{
    public class SubjectSpecification : BaseSpecification<Subject>
    {
        public SubjectSpecification(SubjectParams subjectParams)
            : base(x => (string.IsNullOrEmpty(subjectParams.Search)
            || x.Name.ToLower().Contains(subjectParams.Search)
            || x.Code.ToLower().Contains(subjectParams.Search)) &&
            (!subjectParams.majorId.HasValue || x.MajorSubjects.Any(ms => ms.MajorId == subjectParams.majorId))
            )
        {    
            AddInclude(x => x.MajorSubjects);
            AddInclude(x => x.Majors);
            ApplyPaging(subjectParams.PageSize * (subjectParams.PageIndex - 1),
                subjectParams.PageSize);
            AddOrderBy(x => x.Code);
        }
        public SubjectSpecification(int id)
            : base(x => x.Id == id)
        {
            AddInclude(x => x.MajorSubjects);
            AddInclude(x => x.Majors);
        }
    }
}
