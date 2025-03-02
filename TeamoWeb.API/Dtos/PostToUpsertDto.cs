using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class PostToUpsertDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public PostPrivacy Privacy { get; set; }
    }
}
