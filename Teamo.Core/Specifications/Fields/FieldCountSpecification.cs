using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Fields
{
    public class FieldCountSpecification : BaseSpecification<Field>
    {
        public FieldCountSpecification(FieldParams fieldParams)
            : base(x => string.IsNullOrEmpty(fieldParams.Search)
                || x.Name.ToLower().Contains(fieldParams.Search)
                || x.Description.ToLower().Contains(fieldParams.Search))
        {
        }
    }
}