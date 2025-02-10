using Teamo.Core.Entities.Identity;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class GroupToAddDto
    {
        public required string Name { get; set; }
        public required string Title { get; set; }
        public int SemesterId { get; set; }
        public string? Description { get; set; }
        public int MaxMember { get; set; }
        public int FieldId { get; set; }
        public int SubjectId { get; set; }
        public int? CreatedByUserId { get; set; } 
    }
}
