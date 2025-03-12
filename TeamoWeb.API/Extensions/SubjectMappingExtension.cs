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

        public static Subject ToEntity(this SubjectDto subjectDto, Subject? subject = null)
        {
            
            //Create subject
            if(subject == null)
            {
                return new Subject{
                    Name = subjectDto.Name,
                    Code = subjectDto.Code,
                    Description = subjectDto.Description,
                    ImgUrl = subjectDto.ImgUrl,
                    CreatedDate = subjectDto.CreatedDate,
                    Status = SubjectStatus.Active
                };
            }

            //Update subject
            subject.Name = string.IsNullOrEmpty(subjectDto.Name) ? subject.Name : subjectDto.Name;
            subject.Description = string.IsNullOrEmpty(subjectDto.Description) ? subject.Description : subjectDto.Description;
            subject.ImgUrl = string.IsNullOrEmpty(subjectDto.ImgUrl) ? subject.ImgUrl : subjectDto.ImgUrl;

            return subject;
        }
    }
}