using Teamo.Core.Entities.Identity;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class GroupDto
    {
        public int Id { get; set; } 
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? SemesterName { get; set; }
        public string? Description { get; set; }
        public string? ImgUrl { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? CreatedByUserName { get; set; }
        public int MaxMember { get; set; }
        public GroupStatus Status { get; set; }
        public string? FieldName { get; set; }
        public string? SubjectCode { get; set; }
        public int TotalMembers { get; set; }
        public int TotalGroupPositions { get; set; }
        public int TotalApplications { get; set; }
        public List<GroupMemberDto>? GroupMembers { get; set; }
        public List<GroupPositionDto>? GroupPositions { get; set; }
        public List<ApplicationDto?>? Applications { get; set; }
    }
}
