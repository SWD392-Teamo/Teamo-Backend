using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
                        Id = s.Id,
                        Name = s.Name,
                        Type = s.Type
                    }).ToList() ?? new List<SkillDto>(),
                }).ToList() ?? new List<GroupPositionDto>(),

                Applications = (group.Applications != null) ?
                    group.Applications.Select(a => a.ToDto()).ToList() : new List<ApplicationDto?>()
            };
        }

        public static Group? ToEntity (this GroupToAddDto groupDto)
        {
            if (groupDto == null) return null;
            return new Group
            {
                Name = groupDto.Name,
                Title = groupDto.Title,
                Description = groupDto.Description,
                SemesterId = groupDto.SemesterId,
                MaxMember = groupDto.MaxMember,
                FieldId = groupDto.FieldId, 
                SubjectId = groupDto.SubjectId,
            };
        }
    }
}
