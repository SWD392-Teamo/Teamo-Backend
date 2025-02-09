using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Skills
{
    public class SkillSpecification : BaseSpecification<Skill>
    {
        public SkillSpecification(int id) : base(x => x.Id == id)
        {
        }

        public SkillSpecification(SkillParams skillParams)
            : base(x => string.IsNullOrEmpty(skillParams.Search)
                        || x.Name.ToLower().Contains(skillParams.Search)
                        || x.Type.ToLower().Contains(skillParams.Search)
                        || skillParams.StudentId.HasValue)
        {
            AddOrderBy(s => s.Name);
        }
    }
}