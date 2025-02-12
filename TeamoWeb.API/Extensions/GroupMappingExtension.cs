using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Teamo.Core.Entities;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class GroupMappingExtension
    {
        /**
         * Mapping Group to GroupDto
         */
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
                        Position = gm.GroupPosition?.Name,
                        Role = gm.Role
                    }).ToList() ?? new List<GroupMemberDto>(),

                GroupPositions = group.GroupPositions?
                .Select(gp => new GroupPositionDto
                {
                    Id = gp.Id,
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

        /**
         * Mapping GroupToUpsertDto to Group
         */
        public static Group ToEntity (this GroupToUpsertDto groupDto, Group? group = null)
        {
            // for insert
            if (group == null)
            {
                if (string.IsNullOrEmpty(groupDto.Name) || string.IsNullOrEmpty(groupDto.Title) ||
                    groupDto.SemesterId == null || groupDto.MaxMember == null ||
                    groupDto.FieldId == null || groupDto.SubjectId == null || groupDto.GroupPositions.Any(gp => gp.Count == null))
                {
                    throw new ArgumentException("All required fields must be provided when adding a new group.");
                }
                return new Group
                {
                    Name = groupDto.Name,
                    Title = groupDto.Title,
                    Description = groupDto.Description,
                    SemesterId = groupDto.SemesterId.Value,
                    MaxMember = groupDto.MaxMember.Value,
                    FieldId = groupDto.FieldId.Value,
                    SubjectId = groupDto.SubjectId.Value,
                    GroupPositions = groupDto.GroupPositions
                    .Select(gp => new GroupPosition
                    {
                        Name = gp.Name,
                        Count = (int) gp.Count,
                        GroupPositionSkills = gp.SkillIds
                        .Select(skillId => new GroupPositionSkill
                        {
                            SkillId = skillId
                        }).ToList()
                    }).ToList()
                }; 
            }

            //for update
            group.Name = string.IsNullOrEmpty(groupDto.Name) ? group.Name : groupDto.Name;
            group.Title = string.IsNullOrEmpty(groupDto.Title) ? group.Title : groupDto.Title;
            group.Description = string.IsNullOrEmpty(groupDto.Description) ? group.Description : groupDto.Description;
            group.SemesterId = groupDto.SemesterId ?? group.SemesterId;
            group.MaxMember = groupDto.MaxMember ?? group.MaxMember;
            group.FieldId = groupDto.FieldId ?? group.FieldId;
            group.SubjectId = groupDto.SubjectId ?? group.SubjectId;
            group.Status = groupDto.Status ?? group.Status;

            return group;
        }

        public static GroupPosition ToEntity (this GroupPositionToAddDto groupPositionDto, GroupPosition? groupPosition = null)
        {
            // for add
            if (groupPosition == null)
            {
                if (string.IsNullOrEmpty(groupPositionDto.Name) || groupPositionDto.Count == null)
                {
                    throw new ArgumentException("All required fields must be provided when adding a position to group.");
                }
                return new GroupPosition
                {
                    GroupId = groupPositionDto.GroupId,
                    Name = groupPositionDto.Name,
                    Count = groupPositionDto.Count.Value,
                    GroupPositionSkills = groupPositionDto.SkillIds
                .Select(sId => new GroupPositionSkill
                {
                    SkillId = sId,
                }).ToList()
                };
            }

            // for update
            groupPosition.Name = string.IsNullOrEmpty(groupPositionDto.Name) ? groupPosition.Name : groupPositionDto.Name;
            groupPosition.Count = groupPositionDto.Count ?? groupPosition.Count;
            groupPosition.Status = groupPositionDto.Status ?? groupPosition.Status;
            return groupPosition;
        }
    }
}
