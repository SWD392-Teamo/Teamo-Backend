namespace TeamoWeb.API.Dtos
{
    public class SubjectToUpsertDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
    }
}
