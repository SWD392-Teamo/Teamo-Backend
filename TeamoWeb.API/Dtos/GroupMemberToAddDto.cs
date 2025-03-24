using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class GroupMemberToAddDto
    {
        public int? StudentId { get; set; }
        public GroupMemberRole? Role { get; set; }
        public IEnumerable<int>? GroupPositionIds { get; set; }
    }
}
