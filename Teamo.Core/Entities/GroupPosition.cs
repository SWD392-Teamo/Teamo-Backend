using System.ComponentModel.DataAnnotations.Schema;

namespace Teamo.Core.Entities
{
    public class GroupPosition : BaseEntity
    {
        public required int GroupId { get; set; }
        public Group Group { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public required string Name { get; set; }
        public required int Count { get; set; }
        [Column(TypeName = "varchar(50)")]
        public required string Status { get; set; } = GroupPositionStatus.Open.ToString();
        public IReadOnlyList<GroupPositionSkill> GroupPositionSkills { get; set; }
    }
}