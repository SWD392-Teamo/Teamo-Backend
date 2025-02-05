namespace TeamoWeb.API.Dtos
{
    public class ApplicationDto
    {
        public int Id { get; set; }
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public string? ImgUrl { get; set; }
        public DateTime RequestTime { get; set; } = DateTime.Now;
        public string? RequestContent { get; set; }
        public string? GroupPositionName { get; set; }
        public string? Status { get; set; }
    }
}
