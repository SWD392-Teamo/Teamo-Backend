using Teamo.Core.Entities;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class MajorMappingExtension
    {
        //Display major
        public static MajorDto? ToDto(this Major? major)
        {
            if (major == null) return null;
            return new MajorDto
            {
                Id = major.Id,
                Code = major.Code,
                Name = major.Name,
                CreatedDate = major.CreatedDate,
                Subjects =(major.Subjects != null) ? 
                    major.Subjects.Select(subject => subject.ToDto()).ToList() : null
            };
        }
    }
}