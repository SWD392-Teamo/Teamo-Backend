using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Skills
{
    public class SkillSpecification : BaseSpecification<Skill>
    {
        public SkillSpecification(int id) : base(x => x.Id == id)
        {
        }

        public SkillSpecification(string name) : base(x => x.Name.ToLower().Equals(name.ToLower()))
        {
        }

        public SkillSpecification(SkillParams skillParams)
            : base(x => (string.IsNullOrEmpty(skillParams.Search)
                        || x.Name.ToLower().Contains(skillParams.Search)
                        || x.Type.ToLower().Contains(skillParams.Search))
                        &&(skillParams.StudentId.HasValue 
                        || !skillParams.StudentId.HasValue))
        {
            AddOrderBy(s => s.Name);
            if (skillParams.IsPaginated)
            {
                ApplyPaging(skillParams.PageSize * (skillParams.PageIndex - 1),
                skillParams.PageSize);
            }
        }
    }
}