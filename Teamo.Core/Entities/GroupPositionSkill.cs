namespace Teamo.Core.Entities
{
    public class GroupPositionSkill : BaseEntity
    {
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
        public int GroupPositionId { get; set; }
        public GroupPosition GroupPosition { get; set; }
    }
}