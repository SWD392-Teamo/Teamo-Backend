using Microsoft.Extensions.Configuration;
using Teamo.Core.Interfaces.Services;
using System.Text;
using Newtonsoft.Json;
using Teamo.Infrastructure.Models;
using Teamo.Infrastructure.Models.ContentResponse;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Services
{
    public class AgentService : IAgentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private const string SYSTEM_CONTEXT = @"You are an AI advisor for TEAMO - a student team formation platform at FPT University. 
        Your role is to help students find, form, and manage effective project teams based on their skills, majors, and subjects. 
        Consider the platform's features: group discovery, skill-based matching, application management, and profile-based team building.
        If the user gives unrelated and irrelevant information to the context of the conversation, then tell the user that it is outside the scope of the discussion.
        Please give short and concise answer.";

        public AgentService(IConfiguration config, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> GenerateTeamRecommendationsAsync(string userPrompt)
        {
            var prompt = $@"{SYSTEM_CONTEXT}
                As a team formation advisor for an FPT University student project:
                Project Description and Required Skills: {userPrompt} - [User answer here, check for relevancy]
                
                Please provide detailed recommendations for:
                1. Optimal team size considering the project scope and FPT University's typical project requirements
                2. Key roles needed, matching FPT University's majors and specializations
                3. Skill distribution across team members, considering both technical and soft skills
                4. Team dynamics considerations for effective student collaboration
                
                Format your response in a clear, structured manner suitable for students.";
    
            return await GenerateResponseAsync(prompt);
        }
    
        public async Task<string> GenerateProjectSuggestionsAsync(string userPrompt, IReadOnlyList<string> currentSkills, string groups)
        {
            var prompt = $@"{SYSTEM_CONTEXT}
                Based on an FPT University student's profile:
                Skills and Interests: {userPrompt} - [User answer here, check for relevancy]
                User's skills found in profile that might be relevant to the interest: {string.Join(", ", currentSkills)}
                
                Available Groups/Projects: {groups}

                Suggest suitable projects that would:
                1. Match their current skill level in the context of FPT University courses
                2. Allow skill growth aligned with their major's requirements
                3. Align with their interests while maintaining academic relevance
                4. Consider typical FPT University project scopes and requirements
                5. Suggest the groups/projects suitable for them with the link format: 
                {_config["ClientApp"]}/groups/details/groupId hide the long link with an alias like group name
                
                For each suggestion, provide specific examples and potential team roles.";
    
            return await GenerateResponseAsync(prompt);
        }
    
        public async Task<string> GenerateSkillImprovementPlanAsync(string userPrompt, IReadOnlyList<string> currentSkills)
        {
            var prompt = $@"{SYSTEM_CONTEXT}
                Create a skill development plan for an FPT University student:
                Skills and Target Role: {userPrompt} - [User answer here, check for relevancy]
                User's skills found in profile that might be relevant to the target: {string.Join(", ", currentSkills)}
                
                Provide:
                1. Skills gap analysis based on industry and academic requirements
                2. Learning priorities aligned with FPT University's curriculum
                3. Recommended learning resources, including university resources, online courses and external materials (with links)
                4. Timeline estimation considering academic semester schedule
                
                Focus on both technical and soft skills needed for effective team collaboration.";
    
            return await GenerateResponseAsync(prompt);
        }
    
        private async Task<string> GenerateResponseAsync(string prompt)
        {
            try
            {
                var request = new ContentRequest
                {
                    contents = new[]
                    {
                        new Models.Content
                        {
                            parts = new[]
                            {
                                new Models.Part { text = prompt }
                            }
                        }
                    }
                };
    
                string jsonRequest = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                
                var url = $"{_config["Gemini:BaseUrl"]}?key={_config["Gemini:ApiKey"]}";
                var response = await _httpClient.PostAsync(url, content);
    
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var geminiResponse = JsonConvert.DeserializeObject<ContentResponse>(jsonResponse);
                    return geminiResponse.Candidates[0].Content.Parts[0].Text;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                return $"Unable to generate response: {ex.Message}";
            }
        }
    }
}