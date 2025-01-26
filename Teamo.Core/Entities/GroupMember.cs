using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Attributes;

namespace Teamo.Core.Entities
{
    public class GroupMember : BaseEntity
    {
        public required int GroupId { get; set; }
        public Group Group { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public required string StudentId { get; set; }
        public required string GroupPositionId { get; set; }
        public GroupPosition GroupPosition { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public required string Role { get; set; } = GroupMemberRole.Member.ToString();
    }
}