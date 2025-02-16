namespace TeamoWeb.API.Dtos
{
    public class ProfileUpdateDto
    {
        public string? Description { get; set; }
        public IReadOnlyList<LinkDto?>? Link { get; set; }
        public IReadOnlyList<StudentSkillDto?>? StudentSkill { get; set; }
    }
}