using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class SemesterToUpsertDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
