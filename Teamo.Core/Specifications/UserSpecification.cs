using Teamo.Core.Entities.Identity;

namespace Teamo.Core.Specifications
{
    public class UserSpecification : BaseSpecification<User>
    {
        public UserSpecification(int id)
            : base(x => x.Id.Equals(id))
        {
            AddInclude(x => x.Major);
            AddInclude(x => x.Skills);
            AddInclude(x => x.Links);
        }

        public UserSpecification(string email)
            : base(x => x.Email.Equals(email))
        {
            AddInclude(x => x.Major);
            AddInclude(x => x.Skills);
            AddInclude(x => x.Links);
        }
    }
}
