using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Subjects
{
    public class SubjectSpecification : BaseSpecification<Subject>
    {
        public SubjectSpecification(SubjectParams subjectParams)
            : base(x => (string.IsNullOrEmpty(subjectParams.Search)
            || x.Name.ToLower().Contains(subjectParams.Search)
            || x.Code.ToLower().Contains(subjectParams.Search)) &&
            (!subjectParams.MajorId.HasValue || subjectParams.MajorId.HasValue))
        {    
        }
        
        public SubjectSpecification(int id)
            : base(x => x.Id == id)
        {
        }
    }
}
