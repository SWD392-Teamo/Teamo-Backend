using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Majors
{
    public class MajorSpecification : BaseSpecification<Major>
    {
        public MajorSpecification(MajorSpecParams majorSpecParams)
            : base(x => string.IsNullOrEmpty(majorSpecParams.Search)
                        || x.Code.ToLower().Contains(majorSpecParams.Search)
                        || x.Name.ToLower().Contains(majorSpecParams.Search))
        {
        }

        public MajorSpecification(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.Subjects);
        }
    }
}