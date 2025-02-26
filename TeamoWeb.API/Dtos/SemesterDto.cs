namespace TeamoWeb.API.Dtos
{
    public class SemesterDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Status { get; set; }
    }
}