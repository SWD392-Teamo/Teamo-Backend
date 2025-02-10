using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupPositionSpecification : BaseSpecification<GroupPosition>
    {
        public GroupPositionSpecification(int id) : base(x => x.Id == id)
        {
        }
    }
}