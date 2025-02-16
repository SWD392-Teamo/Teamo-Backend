using Teamo.Core.Entities;
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

                return new Semester
                {
                    Name = dto.Name,
                    Code = dto.Code,
                    StartDate = dto.StartDate.Value,
                    EndDate = dto.EndDate.Value
                };
            }

            // for updating
            semester.Name = string.IsNullOrEmpty(dto.Name) ? dto.Name : semester.Name;
            semester.Code = string.IsNullOrEmpty(dto.Code) ? dto.Code : semester.Code;
            semester.StartDate = dto.StartDate ?? semester.StartDate;
            semester.EndDate = dto.EndDate ?? semester.EndDate;

            return semester;
        }
    }
}
