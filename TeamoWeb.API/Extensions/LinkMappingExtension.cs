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

        public static Link ToEntity(this LinkToUpsertDto linkDto, Link? link = null)
        {
            //Add link
            if(link == null)
            {
                return new Link{
                    Name = linkDto.Name,
                    Url = linkDto.Url,
                    StudentId = linkDto.StudentId
                };
            }

            //Update link
            link.Name = string.IsNullOrEmpty(linkDto.Name) ? link.Name : linkDto.Name;
            link.Url = string.IsNullOrEmpty(linkDto.Url) ? link.Url : linkDto.Url;

            return link;
        }
    }
}