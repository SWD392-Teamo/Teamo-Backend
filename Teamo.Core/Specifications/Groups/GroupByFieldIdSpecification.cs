using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupByFieldIdSpecification : BaseSpecification<Group>
    {
        public GroupByFieldIdSpecification(int fieldId) : base(x => x.FieldId == fieldId)
        {
            
        }
    }
}