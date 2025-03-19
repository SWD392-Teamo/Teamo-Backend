using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class PostToUpsertDto
    {
        public IFormFile? Document { get; set; }
        public string? Content { get; set; } 
        public PostPrivacy? Privacy { get; set; } 
    }
}
