namespace TeamoWeb.API.Dtos
{
    public class GroupPositionToAddDto
    {
        public required string Name { get; set; }
        public int Count { get; set; }
        public IEnumerable<int> SkillIds { get; set; } = Enumerable.Empty<int>();
    }
}
