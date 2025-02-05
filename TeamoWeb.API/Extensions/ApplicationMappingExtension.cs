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
                StudentName = application.SrcStudent.FirstName + " " + application.SrcStudent.LastName,
                StudentEmail = application.SrcStudent.Email,
                ImgUrl= application.SrcStudent.ImgUrl,
                RequestTime = application.RequestTime,
                RequestContent = application.RequestContent,
                Status = application.Status.ToString(),
                GroupPositionName = application.GroupPosition.Name,
            };
        }
    }
}