using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
    public class GroupPosition : BaseEntity
    {
        public required int GroupId { get; set; }
        public Group Group { get; set; }
        public required string Name { get; set; }
        public required int Count { get; set; }
        public required GroupPositionStatus Status { get; set; }
        public List<Skill> Skills { get; set; }
        public List<GroupPositionSkill> GroupPositionSkills { get; set; }
    }
}