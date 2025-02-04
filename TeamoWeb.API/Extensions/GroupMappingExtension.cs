using Teamo.Core.Entities;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class GroupMappingExtension
    {
        public static GroupDto? ToDto (this Group? group)
        {
            if (group == null) return null;
            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Title = group.Title,
                Description = group.Description,
                SemesterName = group.Semester.Name,
                CreatedAt = group.CreatedAt,
                CreatedByUserName = group.CreatedByUser.FirstName + " " + group.CreatedByUser.LastName,
                MaxMember = group.MaxMember,
                Status = group.Status,
                FieldName = group.Field.Name,
                SubjectCode = group.Subject.Code,

                GroupMembers = group.GroupMembers?
                    .Select(gm => new GroupMemberDto
                    {
                        MemberName = gm.Student.FirstName + " " + gm.Student.LastName,
                        MemberEmail = gm.Student.Email,
                        ImgUrl = gm.Student.ImgUrl,
                        Position = gm.GroupPosition.Name,
                        Role = gm.Role
                    }).ToList() ?? new List<GroupMemberDto>(),

                GroupPositions = group.GroupPositions?
                .Select(gp => new GroupPositionDto
                {
                    Name = gp.Name,
                    Count = gp.Count,
                    Status = gp.Status,

                    Skills = gp.Skills?
                    .Select(s => new SkillDto
                    {
                        Name = s.Name,
                        Type = s.Type
                    }).ToList() ?? new List<SkillDto>(),
                }).ToList() ?? new List<GroupPositionDto>(),

                Applications = group.Applications?
                .Select(a => new ApplicationDto
                {
                    StudentName = a.SrcStudent.FirstName + " " + a.SrcStudent.LastName,
                    StudentEmail = a.SrcStudent.Email,
                    ImgUrl= a.SrcStudent.ImgUrl,
                    RequestTime = a.RequestTime,
                    RequestContent = a.RequestContent,
                    Status = a.Status,
                    GroupPositionName = a.GroupPosition.Name,
                }).ToList() ?? new List<ApplicationDto>(),
            };
        }
    }
}
