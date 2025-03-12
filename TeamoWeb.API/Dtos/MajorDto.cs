using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class MajorDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ImgUrl { get; set; }
        public MajorStatus Status { get; set; }
        public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public IReadOnlyList<SubjectDto?>? Subjects {get; set; }
    }
}