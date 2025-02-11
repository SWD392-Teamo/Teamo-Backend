using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Links
{
    public class LinkSpecification : BaseSpecification<Link>
    {
        public LinkSpecification(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.Student);
        }
    }
}