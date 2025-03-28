using Teamo.Core.Entities;
using Teamo.Core.Specifications.Skills;

namespace Teamo.Core.Interfaces
{
    public interface ISkillService
    {
        Task<Skill> GetSkillByIdAsync(int id);
        Task<IReadOnlyList<Skill>> GetSkillsAsync(SkillParams skillParams);
        Task<int> CountSkillsAsync(SkillParams skillParams);
        Task<Skill> CreateSkillAsync(Skill skill);
        Task<bool> UpdateSkillAsync(Skill skill);
        Task<bool> DeleteSkillAsync(Skill skill);
        Task<bool> CheckDuplicateSkillAsync(string name);
    }
}