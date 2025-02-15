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

        public static Major toEntity(this MajorToAddDto majorDto, Major? major = null)
        {
            if(major == null)
            {
                if (string.IsNullOrEmpty(majorDto.Code) || string.IsNullOrEmpty(majorDto.Name))
                    throw new ArgumentException("All required fields must be provided when creating a new major.");
                return new Major
                {
                    Code = majorDto.Code,
                    Name = majorDto.Name
                };
            }

            major.Code = string.IsNullOrEmpty(majorDto.Code) ? major.Code : majorDto.Code;
            major.Name = string.IsNullOrEmpty(majorDto.Name) ? major.Name : majorDto.Name;
            return major;
        }
    }
}