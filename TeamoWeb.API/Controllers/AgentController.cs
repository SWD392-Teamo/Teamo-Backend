using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Groups;
using Teamo.Core.Specifications.Semesters;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Extensions;

namespace TeamoWeb.API.Controllers
{
    public class AgentController : BaseApiController
    {
        private readonly IAgentService _agentService;
        private readonly IProfileService _profileService;
        private readonly IGroupService _groupService;
        private readonly IGenericRepository<Semester> _semesterRepo;

        public AgentController(
            IAgentService agentService, 
            IProfileService profileService, 
            IGroupService groupService,
            IGenericRepository<Semester> semesterRepo)
        {
            _agentService = agentService;
            _profileService = profileService;
            _groupService = groupService;
            _semesterRepo = semesterRepo;
        }

        [HttpPost("team-recommendations")]
        public async Task<IActionResult> GenerateTeamRecommendations([FromBody] string prompt)
        {
            var result = await _agentService.GenerateTeamRecommendationsAsync(prompt);
            if (result == null) return BadRequest("Failed to generate team recommendations");
            return Ok(result);
        }

        [HttpPost("project-suggestions")]
        public async Task<IActionResult> GenerateProjectSuggestions([FromBody] string prompt)
        {
            // Get the current user's skills
            List<string> currentSkills = await GetUserCurrentSkill();

            // Get ongoing semester
            var semSpec = new SemesterSpecification();
            var semester = await _semesterRepo.GetEntityWithSpec(semSpec);

            // Get all recruiting groups in the ongoing semester
            var spec = new GroupSuggestionSpecification(semester.Id);
            var groups = await _groupService.GetGroupsAsync(spec) ?? new List<Group>();
            var groupDtos = groups.Any() 
                ? groups.Select(g => g.ToSuggestionDto()).ToList() 
                : new List<GroupSuggestionDto?>();

            // Serialize the group list to JSON
            var groupJson = JsonConvert.SerializeObject(groupDtos);

            // Generate project suggestions using the current user's skills
            var result = await _agentService.GenerateProjectSuggestionsAsync(prompt, currentSkills, groupJson);
            if (result == null) return BadRequest("Failed to generate project suggestions");
            return Ok(result);
        }

        [HttpPost("skill-improvement")]
        public async Task<IActionResult> GenerateSkillImprovement([FromBody] string prompt)
        {
            // Get the current user's skills
            List<string> currentSkills = await GetUserCurrentSkill();
            
            // Generate skill improvement plan using the current user's skills
            var result = await _agentService.GenerateSkillImprovementPlanAsync(prompt, currentSkills);
            if (result == null) return BadRequest("Failed to generate skill improvement");
            return Ok(result);
        }

        private async Task<List<string>> GetUserCurrentSkill()
        {
            // Get the current user's skills
            var userId = User.GetId();
            var profile = await _profileService.GetProfileAsync(userId);
            var currentSkills = profile.Skills.Select(s => s.Name).ToList();
            return currentSkills;
        }
    }
}