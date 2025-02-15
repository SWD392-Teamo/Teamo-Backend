using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Semesters
{
    public class SemesterSpecification : BaseSpecification<Semester>
    {
        public SemesterSpecification(SemesterParams semesterParams)
            :base(x => (!string.IsNullOrEmpty(semesterParams.Search) || semesterParams.Search.ToLower() == x.Name.ToLower() 
            || semesterParams.Search.ToLower() == x.Code.ToLower()) 
            && (!semesterParams.Status.HasValue || semesterParams.Status == x.Status))
        {
        }
    }
}
