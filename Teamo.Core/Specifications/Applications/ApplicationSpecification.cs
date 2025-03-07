using Teamo.Core.Constants;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

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
                        || x.Status == ParseStatus<ApplicationStatus>(appParams.Status)))
        {
            AddInclude(x => x.Group);
            AddInclude(x => x.Student);
            AddInclude(x => x.GroupPosition);

            ApplyPaging(appParams.PageSize * (appParams.PageIndex - 1),
                appParams.PageSize);
            
            if(!string.IsNullOrEmpty(appParams.Sort))
            {
                switch (appParams.Sort)
                {
                    case SortOptions.DateAsc:
                        AddOrderBy(x => x.RequestTime);
                        break;
                    case SortOptions.DateDesc:
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