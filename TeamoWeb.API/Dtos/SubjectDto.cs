using Teamo.Core.Entities;

namespace TeamoWeb.API.Dtos
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateOnly CreatedDate { get; set; }
        public List<Group>? Groups { get; set; }
    }
}