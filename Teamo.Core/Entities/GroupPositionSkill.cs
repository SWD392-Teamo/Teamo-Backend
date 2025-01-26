namespace Teamo.Core.Entities
{
    public class GroupPositionSkill : BaseEntity
    {
        public required int SkillId { get; set; }
        public Skill Skill { get; set; }
        public required int GroupPositionId { get; set; }
        public GroupPosition GroupPosition { get; set; }
    }
}