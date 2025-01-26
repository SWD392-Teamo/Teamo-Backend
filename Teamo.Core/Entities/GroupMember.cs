using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
    public class GroupMember : BaseEntity
    {
        public required int GroupId { get; set; }
        public Group Group { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public required int StudentId { get; set; }
        public User Student { get; set; }
        public required string GroupPositionId { get; set; }
        public GroupPosition GroupPosition { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public required GroupMemberRole Role { get; set; } 
    }
}