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
                Id = application.Id,
                GroupId = application.GroupId,
                GroupName = application.Group.Name,
                StudentName = application.Student.FirstName + " " + application.Student.LastName,
                StudentEmail = application.Student.Email,
                ImgUrl= application.Student.ImgUrl,
                RequestTime = application.RequestTime,
                RequestContent = application.RequestContent,
                DocumentUrl = application.DocumentUrl,
                GroupPositionName = (application.GroupPosition == null) ? null 
                    : application.GroupPosition.Name,
                Status = application.Status.ToString()
            };
        }

        public static Application ToEntity(this ApplicationToUpsertDto appDto, Application? app = null)
        {
            //Create application
            if (app == null)
            {
                return new Application{
                    GroupId = appDto.GroupId,
                    StudentId = appDto.StudentId,
                    RequestTime = appDto.RequestTime,
                    RequestContent = appDto.RequestContent ?? "I would like to become a member of this group.",
                    DocumentUrl = appDto.DocumentUrl,
                    GroupPositionId = appDto.GroupPositionId,
                    Status = ApplicationStatus.Requested
                };
            }
            
            //Review application
            Enum.TryParse(appDto.Status, out ApplicationStatus status);
            app.Status = status;

            return app;
        }
    }
}