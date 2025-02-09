using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Applications
{
    public class ApplicationSpecification : BaseSpecification<Application>
    {
        public ApplicationSpecification(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.Group);
            AddInclude(x => x.Student);
            AddInclude(x => x.GroupPosition);
        }

        //Spec to retrieve user's sent applications
        public ApplicationSpecification(ApplicationParams appParams)
            : base(x => (x.StudentId == appParams.StudentId)
                        && (string.IsNullOrEmpty(appParams.Status)
                        || x.Status.ToString().ToLower().Equals(appParams.Status.ToLower())))
        {
            AddInclude(x => x.Group);
            AddInclude(x => x.Student);
            AddInclude(x => x.GroupPosition);
            
            if(!string.IsNullOrEmpty(appParams.Sort))
            {
                switch (appParams.Sort)
                {
                    case "dateAsc":
                        AddOrderBy(x => x.RequestTime);
                        break;
                    case "dateDesc":
                        AddOrderByDescending(x => x.RequestTime);
                        break;
                    default:
                        AddOrderByDescending(x => x.RequestTime);
                        break;
                }
            }
        }
    }
}