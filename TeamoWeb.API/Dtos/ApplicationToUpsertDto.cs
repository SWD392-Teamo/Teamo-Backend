namespace TeamoWeb.API.Dtos
{
    public class ApplicationToUpsertDto
    {
        public int GroupId { get; set; }
        public int StudentId { get; set; }
        public DateTime RequestTime { get; set; } = DateTime.Now;
        public string? DocumentUrl { get; set; }
        public string? RequestContent { get; set; }
        public int GroupPositionId { get; set; }
        public string? Status { get; set; }
    }
}
