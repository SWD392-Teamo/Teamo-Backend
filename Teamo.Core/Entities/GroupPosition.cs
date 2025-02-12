using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
    public class GroupPosition : BaseEntity
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public required string Name { get; set; }
        public required int Count { get; set; }
        public GroupPositionStatus Status { get; set; } = GroupPositionStatus.Open;
        public List<Skill> Skills { get; set; }
        public List<GroupPositionSkill> GroupPositionSkills { get; set; }
    }
}