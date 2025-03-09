using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class GroupSuggestionDto
    {
        public int Id { get; set; } 
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FieldName { get; set; }
        public List<GroupPositionDto>? GroupPositions { get; set; }
    }
}
