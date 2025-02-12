using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
    public class GroupMember : BaseEntity
    {
        public required int GroupId { get; set; }
        public Group Group { get; set; }
        public required int StudentId { get; set; }
        public User Student { get; set; }
        public int? GroupPositionId { get; set; }
        public GroupPosition GroupPosition { get; set; }
        public GroupMemberRole Role { get; set; } 
    }
}