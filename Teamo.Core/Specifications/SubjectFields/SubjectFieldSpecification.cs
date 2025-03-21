using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.SubjectFields
{
    public class SubjectFieldSpecification : BaseSpecification<SubjectField>
    {
        public SubjectFieldSpecification(SubjectFieldParams subjectFieldParams) 
            : base(x => x.SubjectId == subjectFieldParams.SubjectId)
        {
            AddInclude(x => x.Field);
        }

        public SubjectFieldSpecification(int fieldId) : base(x => x.FieldId == fieldId)
        {
        }
    }
}