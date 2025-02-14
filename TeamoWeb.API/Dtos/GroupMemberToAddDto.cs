namespace TeamoWeb.API.Dtos
{
    public class GroupMemberToAddDto
    {
        public int StudentId { get; set; }
        public IEnumerable<int> GroupPositionIds { get; set; } = new List<int>();
    }
}
