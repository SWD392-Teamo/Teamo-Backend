using Teamo.Core.Entities;

namespace Teamo.Core.Interfaces.Services
{
    public interface IAgentService
    {
        Task<string> GenerateTeamRecommendationsAsync(string userPrompt);
        Task<string> GenerateProjectSuggestionsAsync(string userPrompt, IReadOnlyList<string> currentSkills, string groups);
        Task<string> GenerateSkillImprovementPlanAsync(string userPrompt, IReadOnlyList<string> currentSkills);
    }
}