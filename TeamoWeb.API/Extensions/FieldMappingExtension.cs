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

        public static Field ToEntity(this FieldDto fieldDto)
        {
            //Add field
            return new Field{
                Name = fieldDto.Name,
                Description = fieldDto.Description
            };
        }
    }
}