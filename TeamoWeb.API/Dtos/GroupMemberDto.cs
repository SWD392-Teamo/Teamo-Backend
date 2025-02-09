using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class GroupMemberDto
    {
        public string? MemberName { get; set; }
        public string? MemberEmail { get; set; }
        public string? ImgUrl { get; set; }
        public string? Position { get; set; }
        public GroupMemberRole Role { get; set; }

    }
}
