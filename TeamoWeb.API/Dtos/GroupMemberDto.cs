using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class GroupMemberDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public string? ImgUrl { get; set; }
        public IEnumerable<string> Positions { get; set; } = new List<string>();
        public GroupMemberRole Role { get; set; }

    }
}
