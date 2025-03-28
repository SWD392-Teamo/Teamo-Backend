using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateOnly Dob { get; set; }
        public string? ImgUrl { get; set; }
        public string? Description { get; set; }
        public string? MajorCode { get; set; }
        public UserStatus Status { get; set; }
        public IReadOnlyList<LinkDto?>? Links { get; set; }
        public IReadOnlyList<StudentSkillDto?>? StudentSkills { get; set; }
    }
}