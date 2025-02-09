using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.MajorSubjects
{
    public class MajorSubjectSpecification : BaseSpecification<MajorSubject>
    {
        public MajorSubjectSpecification(int majorId) : base(x => x.MajorId == majorId)
        {
            AddInclude(x => x.Subject);
        }
    }
}