namespace TeamoWeb.API.Dtos
{
    public class ProfileUpdateDto
    {
        public int UserId { get; set; }
        public string? Description { get; set; }
        public LinkDto? Link { get; set; }
        public StudentSkillDto? StudentSkill { get; set; }
    }
}