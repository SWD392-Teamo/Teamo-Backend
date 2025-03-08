using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Core.Specifications.Applications
{
    public class ApplicationGroupSpecification : BaseSpecification<Application>
    {

        public ApplicationGroupSpecification(ApplicationParams appParams)
            : base(x => (x.GroupId == appParams.GroupId)
                    && (string.IsNullOrEmpty(appParams.Status) 
                    ? x.Status == ApplicationStatus.Requested 
                    : x.Status == ParseStatus<ApplicationStatus>(appParams.Status)))
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