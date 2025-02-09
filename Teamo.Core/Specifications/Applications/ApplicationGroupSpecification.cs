using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Applications
{
    public class ApplicationGroupSpecification : BaseSpecification<Application>
    {

        public ApplicationGroupSpecification(ApplicationParams appParams)
            : base(x => (x.GroupId == appParams.GroupId)
                    &&(!appParams.PositionId.HasValue || x.GroupPositionId == appParams.PositionId)
                    &&(string.IsNullOrEmpty(appParams.Status)
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