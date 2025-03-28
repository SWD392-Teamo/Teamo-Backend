using Teamo.Core.Entities;
using Teamo.Core.Specifications.Skills;

namespace Teamo.Core.Specifications.Fields
{
    public class FieldSpecification : BaseSpecification<Field>
    {
        public FieldSpecification(int id) : base(x => x.Id == id)
        {
        }

        public FieldSpecification(string name) : base(x => x.Name.ToLower().Equals(name.ToLower()))
        {
        }

        public FieldSpecification(FieldParams fieldParams)
            : base(x => string.IsNullOrEmpty(fieldParams.Search)
                || x.Name.ToLower().Contains(fieldParams.Search)
                || x.Description.ToLower().Contains(fieldParams.Search))
        {
            AddOrderBy(x => x.Name);
            if (fieldParams.IsPaginated)
            {
                ApplyPaging(fieldParams.PageSize * (fieldParams.PageIndex - 1),
                fieldParams.PageSize);
            }
        }
    }
}