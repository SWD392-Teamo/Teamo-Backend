using System.Net.NetworkInformation;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class SemesterMappingExtension
    {
        public static Semester ToEntity(this SemesterToUpsertDto dto, Semester? semester = null)
        {
            // for adding
            if (semester == null)
            {
                if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Code)
                    || !dto.StartDate.HasValue || !dto.EndDate.HasValue)
                {
                    throw new ArgumentException("All required fields must be provided when adding a new semester.");
                }

                semester = new Semester
                {
                    Name = dto.Name,
                    Code = dto.Code,
                    StartDate = dto.StartDate.Value,
                    EndDate = dto.EndDate.Value
                };
            }
            else
            {
                // for updating
                semester.Name = string.IsNullOrEmpty(dto.Name) ? dto.Name : semester.Name;
                semester.Code = string.IsNullOrEmpty(dto.Code) ? dto.Code : semester.Code;
                semester.StartDate = dto.StartDate ?? semester.StartDate;
                semester.EndDate = dto.EndDate ?? semester.EndDate;
            }
            
            if(semester.StartDate > semester.EndDate)
            {
                throw new ArgumentException("End date must be greater than start date.");
            }

            semester.Status = UpdateStatus(semester.StartDate, semester.EndDate);
            return semester;
        }
        private static SemesterStatus UpdateStatus(DateOnly startDate, DateOnly endDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (startDate > today)
            {
                return SemesterStatus.Upcoming;
            }
            else if (startDate <= today && endDate > today)
            {
                return SemesterStatus.Ongoing;
            }
            else
            {
                return SemesterStatus.Past;
            }
        }

        public static SemesterDto? ToDto(this Semester? semester)
        {
            if(semester == null) return null;
            return new SemesterDto
            {
                Id = semester.Id,
                Name = semester.Name,
                Code = semester.Code,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate,
                Status = semester.Status.ToString()
            };
        }
    }
}
