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
                StudentId = application.SrcStudentId,
                StudentName = application.SrcStudent.FirstName + " " + application.SrcStudent.LastName,
                StudentEmail = application.SrcStudent.Email,
                ImgUrl= application.SrcStudent.ImgUrl,
                RequestTime = application.RequestTime,
                RequestContent = application.RequestContent,
                Status = application.Status.ToString(),
                GroupPositionId = application.GroupPositionId,
                GroupPositionName = application.GroupPosition.Name,
            };
        }
    }
}