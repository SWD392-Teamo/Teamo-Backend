using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Subjects
{
    public class SubjectSpecification : BaseSpecification<Subject>
    {
        public SubjectSpecification(SubjectParams subjectParams)
            : base(x => (string.IsNullOrEmpty(subjectParams.Search)
            || x.Name.ToLower().Contains(subjectParams.Search)
            || x.Code.ToLower().Contains(subjectParams.Search)) &&
            (!subjectParams.MajorId.HasValue || x.MajorSubjects.Any(ms => ms.MajorId == subjectParams.MajorId))
            )
        {    
            AddInclude(x => x.MajorSubjects);
            ApplyPaging(subjectParams.PageSize * (subjectParams.PageIndex - 1),
                subjectParams.PageSize);
        }
        public SubjectSpecification(int id)
            : base(x => x.Id == id)
        {
            AddInclude(x => x.MajorSubjects);
        }
    }
}
