using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
    public class StudentSkill : BaseEntity
    {
        public required int SkillId { get; set; }
        public Skill Skill { get; set; }
        public required int StudentId { get; set; }
        public User Student { get; set; }
        public required StudentSkillLevel Level { get; set; }
    }
}