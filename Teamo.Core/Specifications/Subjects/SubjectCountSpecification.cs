using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Core.Specifications.Subjects
{
    public class SubjectCountSpecification : BaseSpecification<Subject>
    {
        public SubjectCountSpecification(SubjectParams subjectParams)
            : base(x => (string.IsNullOrEmpty(subjectParams.Search)
            || x.Name.ToLower().Contains(subjectParams.Search)
            || x.Code.ToLower().Contains(subjectParams.Search))
            && (string.IsNullOrEmpty(subjectParams.Status)
            || x.Status == ParseStatus<SubjectStatus>(subjectParams.Status)))
        {
        }
    }
}
