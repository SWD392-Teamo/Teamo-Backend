namespace TeamoWeb.API.Dtos
{
    public class MajorToUpsertDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
        public string? SubjectIds { get; set; }
    }
}
