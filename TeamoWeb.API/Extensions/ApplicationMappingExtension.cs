using Teamo.Core.Entities;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class ApplicationMappingExtension
    {
        public static ApplicationDto? ToDto(this Application? application)
        {
            if(application == null) return null;
            return new ApplicationDto{
                Id = application.Id,
                GroupId = application.GroupId,
                StudentId = application.StudentId,
                StudentName = application.Student.FirstName + " " + application.Student.LastName,
                StudentEmail = application.Student.Email,
                ImgUrl= application.Student.ImgUrl,
                RequestTime = application.RequestTime,
                RequestContent = application.RequestContent,
                Status = application.Status.ToString(),
                GroupPositionId = application.GroupPositionId,
                GroupPositionName = application.GroupPosition.Name,
            };
        }
    }
}