using Teamo.Core.Entities;

namespace TeamoWeb.API.Dtos
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImgUrl { get; set; }
        public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string? Status { get; set; }
    }
}