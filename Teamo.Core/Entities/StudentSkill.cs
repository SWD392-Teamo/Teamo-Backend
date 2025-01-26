using System.ComponentModel.DataAnnotations.Schema;

namespace Teamo.Core.Entities
{
    public class StudentSkill : BaseEntity
    {
        public required int SkillId { get; set; }
        public Skill Skill { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public required string StudentId { get; set; }
        [Column(TypeName = "varchar(50)")]
        public required string Level { get; set; }
    }
}