using Teamo.Core.Entities;
using Teamo.Core.Enums;
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
                ImgUrl = subject.ImgUrl,
                CreatedDate = subject.CreatedDate,
                Status = subject.Status.ToString()
            };
        }

        public static Subject ToEntity(this SubjectToUpsertDto subjectDto, Subject? subject = null)
        {
            
            //Create subject
            if(subject == null)
            {
                if (string.IsNullOrEmpty(subjectDto.Code) || string.IsNullOrEmpty(subjectDto.Name))
                    throw new ArgumentException("All required fields must be provided when creating a new major.");
                return new Subject{
                    Name = subjectDto.Name,
                    Code = subjectDto.Code,
                    Description = subjectDto.Description,
                };
            }

            //Update subject
            subject.Name = string.IsNullOrEmpty(subjectDto.Name) ? subject.Name : subjectDto.Name;
            subject.Description = string.IsNullOrEmpty(subjectDto.Description) ? subject.Description : subjectDto.Description;

            return subject;
        }
    }
}