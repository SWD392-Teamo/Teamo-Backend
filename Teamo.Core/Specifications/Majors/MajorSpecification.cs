using Teamo.Core.Entities;
using Teamo.Core.Specifications.Groups;

namespace Teamo.Core.Specifications.Majors
{
    public class MajorSpecification : BaseSpecification<Major>
    {
        public MajorSpecification(MajorSpecParams majorSpecParams)
            : base(x => (string.IsNullOrEmpty(majorSpecParams.Search)
                        || x.Code.ToLower().Contains(majorSpecParams.Search)
                        || x.Name.ToLower().Contains(majorSpecParams.Search)) &&
            (!majorSpecParams.Status.HasValue || majorSpecParams.Status == x.Status))
        {
            ApplyPaging(majorSpecParams.PageSize * (majorSpecParams.PageIndex - 1),
                majorSpecParams.PageSize);
        }

        public MajorSpecification(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.Subjects);
        }
    }
}