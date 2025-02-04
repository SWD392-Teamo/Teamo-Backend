using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class ApplicationDto
    {
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string ImgUrl { get; set; }
        public DateTime RequestTime { get; set; } = DateTime.Now;
        public string RequestContent { get; set; }
        public GroupPositionDto GroupPosition { get; set; }
        public ApplicationStatus Status { get; set; }
    }
}
