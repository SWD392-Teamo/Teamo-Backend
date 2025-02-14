using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class GroupMemberDto
    {
        public int Id { get; set; }
        public string? MemberName { get; set; }
        public string? MemberEmail { get; set; }
        public string? ImgUrl { get; set; }
        public IEnumerable<string> Positions { get; set; } = new List<string>();
        public GroupMemberRole Role { get; set; }

    }
}
