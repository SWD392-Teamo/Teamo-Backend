using Teamo.Core.Entities;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class FieldMappingExtension
    {
        public static FieldDto? ToDto(this Field? field)
        {
            if(field == null) return null;
            return new FieldDto
            {
                Id = field.Id,
                Name = field.Name,
                Description = field.Description
            };
        }

        public static Field ToEntity(this FieldDto fieldDto, Field? field = null)
        {
            //Add field
            if(field == null)
            {
                return new Field{
                    Name = fieldDto.Name,
                    Description = fieldDto.Description
                };
            }

            //Update field
            field.Name = string.IsNullOrEmpty(fieldDto.Name) ? field.Name : fieldDto.Name;
            field.Description = string.IsNullOrEmpty(fieldDto.Description) ? field.Description : fieldDto.Description;

            return field;
        }
    }
}