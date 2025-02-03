namespace TeamoWeb.API.Dtos
{
    public class MajorDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public DateOnly CreatedDate { get; set; }
        public IReadOnlyList<SubjectDto?>? Subjects {get; set; }
    }
}