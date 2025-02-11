using Teamo.Core.Entities;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class LinkMappingExtension
    {
        public static LinkDto? ToDto(this Link? link)
        {
            if(link == null) return null;
            return new LinkDto
            {
                Id = link.Id,
                Name = link.Name,
                Url = link.Url
            };
        }
    }
}