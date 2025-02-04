using Teamo.Core.Entities.Identity;

namespace Teamo.Core.Specifications.Users
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

        public UserSpecification(UserSpecParams userSpecParams)
            : base(x => (string.IsNullOrEmpty(userSpecParams.Search)
                        || x.Code.ToLower().Contains(userSpecParams.Search)
                        || x.FirstName.ToLower().Contains(userSpecParams.Search)
                        || x.LastName.ToLower().Contains(userSpecParams.Search)
                        || x.Email.Contains(userSpecParams.Search)
                        || x.MajorID.Equals(userSpecParams.MajorId))
                        && (string.IsNullOrEmpty(userSpecParams.UserStatus)
                        || x.Status.ToString().ToLower().Equals(userSpecParams.UserStatus.ToLower())))
        {
            AddInclude(x => x.Major);
            AddInclude(x => x.Skills);
            AddInclude(x => x.Links);

            //Sort users alphabetically by first name
            if(!string.IsNullOrEmpty(userSpecParams.Sort)) AddOrderBy(x => x.FirstName);
        }
    }
}
