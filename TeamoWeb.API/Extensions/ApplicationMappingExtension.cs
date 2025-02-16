using Teamo.Core.Entities;
using Teamo.Core.Enums;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class ApplicationMappingExtension
    {
        public static ApplicationDto? ToDto(this Application? application)
        {
            if(application == null) return null;
            return new ApplicationDto{
                GroupName = application.Group.Name,
                StudentName = application.Student.FirstName + " " + application.Student.LastName,
                StudentEmail = application.Student.Email,
                ImgUrl= application.Student.ImgUrl,
                RequestTime = application.RequestTime,
                RequestContent = application.RequestContent,
                GroupPositionName = application.GroupPosition.Name,
                Status = application.Status.ToString()
            };
        }

        public static Application ToEntity(this ApplicationToUpsertDto applicationDto, Application? application = null)
        {
            //Create application
            if (application == null)
            {
                return new Application{
                    GroupId = applicationDto.GroupId,
                    StudentId = applicationDto.StudentId,
                    RequestTime = applicationDto.RequestTime,
                    RequestContent = applicationDto.RequestContent,
                    GroupPositionId = applicationDto.GroupPositionId,
                    Status = ApplicationStatus.Requested
                };
            }
            
            //Review application
            Enum.TryParse(applicationDto.Status, out ApplicationStatus status);
            application.Status = status;

            return application;
        }
    }
}