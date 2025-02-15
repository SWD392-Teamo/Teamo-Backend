using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.SubjectFields
{
    public class SubjectFieldSpecification : BaseSpecification<SubjectField>
    {
        public SubjectFieldSpecification(int? subjectId) : base(x => x.SubjectId == subjectId)
        {
            AddInclude(x => x.Field);
        }
    }
}