using Teamo.Core.Entities;
using Teamo.Core.Specifications.Skills;

namespace Teamo.Core.Interfaces
{
    public interface ISkillService
    {
        Task<Skill> GetSkillByIdAsync(int id);
        Task<IReadOnlyList<Skill>> GetSkillsWithSpecAsync(SkillParams skillParams);
    }
}