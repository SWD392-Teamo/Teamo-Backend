using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
using Teamo.Core.Specifications.Groups;
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
            var groupDto = new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Title = group.Title,
                Description = group.Description,
                ImgUrl = group.ImgUrl,
                SemesterName = group.Semester.Name,
                CreatedAt = group.CreatedAt,
                CreatedByUserName = group.CreatedByUser.FirstName + " " + group.CreatedByUser.LastName,
                MaxMember = group.MaxMember,
                Status = group.Status,
                FieldName = group.Field.Name,
                SubjectCode = group.Subject.Code,
                SubjectId = group.SubjectId,

                GroupMembers = group.GroupMembers?
                    .Select(gm => gm.ToDto()).ToList() ?? new List<GroupMemberDto>(),

                GroupPositions = group.GroupPositions?
                .Where(gp => gp.Status != GroupPositionStatus.Deleted)
                .Select(gp => gp.ToDto()).ToList() ?? new List<GroupPositionDto>(),

                Applications = (group.Applications != null) ?
                    group.Applications.Select(a => a.ToDto()).ToList() : new List<ApplicationDto?>(),

            };

            groupDto.TotalMembers = groupDto.GroupMembers.Count();
            groupDto.TotalGroupPositions = groupDto.GroupPositions.Count();
            groupDto.TotalApplications = groupDto.Applications.Count();

            return groupDto;    
        }

        /**
         * Mapping Group to GroupSuggestionDto
         */
        public static GroupSuggestionDto? ToSuggestionDto (this Group? group)
        {
            if (group == null) return null;
            var groupDto = new GroupSuggestionDto
            {
                Id = group.Id,
                Name = group.Name,
                Title = group.Title,
                Description = group.Description,
                FieldName = group.Field.Name,
                SubjectCode = group.Subject.Code,
                GroupPositions = group.GroupPositions?
                .Where(gp => gp.Status != GroupPositionStatus.Deleted)
                .Select(gp => gp.ToDto()).ToList() ?? new List<GroupPositionDto>(),
            };

            return groupDto;    
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
                    ImgUrl = groupDto.ImgUrl,
                    SemesterId = groupDto.SemesterId.Value,
                    MaxMember = groupDto.MaxMember.Value,
                    FieldId = groupDto.FieldId.Value,
                    SubjectId = groupDto.SubjectId.Value,
                    GroupPositions = groupDto.GroupPositions
                    .Select(gp => gp.ToEntity()).ToList()
                }; 
            }

            //for update
            group.Name = string.IsNullOrEmpty(groupDto.Name) ? group.Name : groupDto.Name;
            group.Title = string.IsNullOrEmpty(groupDto.Title) ? group.Title : groupDto.Title;
            group.Description = string.IsNullOrEmpty(groupDto.Description) ? group.Description : groupDto.Description;
            group.ImgUrl = string.IsNullOrEmpty(groupDto.ImgUrl) ? group.ImgUrl : groupDto.ImgUrl;
            group.SemesterId = groupDto.SemesterId ?? group.SemesterId;
            group.MaxMember = groupDto.MaxMember ?? group.MaxMember;
            group.FieldId = groupDto.FieldId ?? group.FieldId;
            group.SubjectId = groupDto.SubjectId ?? group.SubjectId;
            group.Status = groupDto.Status ?? group.Status;

            return group;
        }

        // Mapping GroupPositionToUpsertDto to GroupPosition Entity

        public static GroupPosition ToEntity (this GroupPositionToUpsertDto groupPositionDto, GroupPosition? groupPosition = null)
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
            groupPosition.GroupPositionSkills = groupPositionDto.SkillIds?
                                                      .Select(sId => new GroupPositionSkill
                                                      {
                                                          SkillId = sId
                                                      }).ToList() ?? groupPosition.GroupPositionSkills;
            return groupPosition;
        }

        // Mapping GroupMemberToAddDto to GroupMember
        public static GroupMember ToEntity(this GroupMemberToAddDto groupMemberToAddDto, GroupMember? groupMember = null)
        {
            if (groupMember == null)
            {
                if(groupMemberToAddDto.StudentId == null)
                    throw new ArgumentException("Student ID is required.");
                return new GroupMember
                {
                    StudentId = groupMemberToAddDto.StudentId.Value,
                    GroupMemberPositions = groupMemberToAddDto.GroupPositionIds?
                                                      .Select(gpId => new GroupMemberPosition
                                                      {
                                                          GroupPositionId = gpId
                                                      }).ToList() ?? new List<GroupMemberPosition>()

                };
            }
            groupMember.Role = groupMemberToAddDto.Role ?? groupMember.Role;
            groupMember.GroupMemberPositions = groupMemberToAddDto.GroupPositionIds?
                                                      .Select(gpId => new GroupMemberPosition
                                                      {
                                                          GroupPositionId = gpId
                                                      }).ToList() ?? groupMember.GroupMemberPositions;
            return groupMember;
        }

        // Mapping GroupMember to GroupMemberDto
        public static GroupMemberDto ToDto(this GroupMember groupMember)
        {
            return new GroupMemberDto
            {
                Id = groupMember.Id,
                GroupId = groupMember.GroupId,
                StudentId = groupMember.StudentId,
                StudentName = groupMember.Student.FirstName + " " + groupMember.Student.LastName,
                StudentEmail = groupMember.Student.Email,
                ImgUrl = groupMember.Student.ImgUrl,
                PositionIds = groupMember.GroupMemberPositions?.Select(p => p.GroupPositionId) ?? [],
                Positions = groupMember.GroupMemberPositions?.Select(gp => gp.GroupPosition.Name) ?? [],
                Role = groupMember.Role
            };
        }

        // Mapping GroupPosition to GroupPositionDto
        public static GroupPositionDto ToDto(this GroupPosition groupPosition)
        {
            return new GroupPositionDto
            {
                Id = groupPosition.Id,
                Name = groupPosition.Name,
                Count = groupPosition.Count,
                Status = groupPosition.Status,

                Skills = groupPosition.GroupPositionSkills?.Select(gps => gps.Skill.ToDto()).ToList() ?? []
            };
        }
    }
}
