using System.Text.Json;
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
                ImgUrl = major.ImgUrl,
                CreatedDate = major.CreatedDate,
                Status = major.Status,
                Subjects =(major.Subjects != null) ? 
                    major.Subjects.Select(subject => subject.ToDto()).ToList() : null
            };
        }

        public static Major toEntity(this MajorToUpsertDto majorDto, Major? major = null)
        {
            var intarray = JsonSerializer.Deserialize<int[]>(majorDto.SubjectIds);

            if(major == null)
            {
                if (string.IsNullOrEmpty(majorDto.Code) || string.IsNullOrEmpty(majorDto.Name))
                    throw new ArgumentException("All required fields must be provided when creating a new major.");
                return new Major
                {
                    Code = majorDto.Code,
                    Name = majorDto.Name,
                    MajorSubjects = intarray?.Select(sId => new MajorSubject{
                        SubjectId = sId
                    }).ToList() ?? new List<MajorSubject>()
                };
            }

            major.Code = string.IsNullOrEmpty(majorDto.Code) ? major.Code : majorDto.Code;
            major.Name = string.IsNullOrEmpty(majorDto.Name) ? major.Name : majorDto.Name;
            major.MajorSubjects = intarray?.Select(sId => new MajorSubject
                {
                    SubjectId = sId
                }).ToList() ?? major.MajorSubjects;
            return major;
        }
    }
}