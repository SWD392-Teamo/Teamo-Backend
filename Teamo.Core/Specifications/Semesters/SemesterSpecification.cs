using Teamo.Core.Entities;
using Teamo.Core.Specifications.Majors;

namespace Teamo.Core.Specifications.Semesters
{
    public class SemesterSpecification : BaseSpecification<Semester>
    {
        public SemesterSpecification(SemesterParams semesterParams)
            :base(x => (string.IsNullOrEmpty(semesterParams.Search) || x.Name.ToLower().Contains(semesterParams.Search)  
            || x.Code.ToLower().Contains(semesterParams.Search)) 
            && (!semesterParams.Status.HasValue || semesterParams.Status == x.Status))
        {
            ApplyPaging(semesterParams.PageSize * (semesterParams.PageIndex - 1),
                semesterParams.PageSize);

            AddOrderByDescending(x => x.StartDate);
        }
    }
}
