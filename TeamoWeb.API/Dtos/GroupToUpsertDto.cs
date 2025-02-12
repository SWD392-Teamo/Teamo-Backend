using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class GroupToUpsertDto
    {
        public string? Name { get; set; }
        public string? Title { get; set; } 
        public int? SemesterId { get; set; }
        public string? Description { get; set; }
        public int? MaxMember { get; set; }
        public int? FieldId { get; set; }
        public int? SubjectId { get; set; }
        public GroupStatus? Status { get; set; }    
        public IEnumerable<GroupPositionToAddDto> GroupPositions { get; set; } = Enumerable.Empty<GroupPositionToAddDto>();
    }
}
