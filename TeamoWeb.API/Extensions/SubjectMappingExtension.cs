using Teamo.Core.Entities;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class SubjectMappingExtension
    {
        //Display subject
        public static SubjectDto? ToDto(this Subject? subject)
        {
            if (subject == null) return null;

            return new SubjectDto
            {
                Id = subject.Id,
                Code = subject.Code,
                Name = subject.Name,
                Description = subject.Description,
                CreatedDate = subject.CreatedDate,
                Groups = subject.Groups,
            };
        }
    }
}