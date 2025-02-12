using Teamo.Core.Entities.Identity;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class GroupMemberToAddDto
    {
        public int GroupId { get; set; }
        public int StudentId { get; set; }
        public int GroupPositionId { get; set; }
    }
}
