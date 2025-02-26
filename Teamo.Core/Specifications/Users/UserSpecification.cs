using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;

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
                        || x.Status == ParseStatus<UserStatus>(userSpecParams.UserStatus)))
        {
            AddInclude(x => x.Major);
            AddInclude(x => x.Skills);
            AddInclude(x => x.Links);

            ApplyPaging(userSpecParams.PageSize * (userSpecParams.PageIndex - 1),
                userSpecParams.PageSize);

            //Sort users alphabetically by first name
            if(!string.IsNullOrEmpty(userSpecParams.Sort)) AddOrderBy(x => x.FirstName);
        }
    }
}
