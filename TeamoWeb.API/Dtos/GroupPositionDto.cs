using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class GroupPositionDto
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public GroupPositionStatus Status { get; set; }
        public List<SkillDto> Skills { get; set; }
    }
}
