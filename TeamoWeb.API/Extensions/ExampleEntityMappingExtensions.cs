using Teamo.Core.Entities;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class ExampleEntityMappingExtensions
    {
        // Create your own mapping extension method for your entity
        // instead of using AutoMapper
        // For Display
        public static ExampleEntityDto? ToDto(this ExampleEntity? exampleEntity)
        {
            if (exampleEntity == null) return null;

            return new ExampleEntityDto
            {
                Name = exampleEntity.Name,
                Age = exampleEntity.Age,
                Pronoun = exampleEntity.Gender.Pronoun
            };
        }

        // For Create
        public static ExampleEntity? ToEntity(this ExampleEntityCreateDto? exampleEntity)
        {
            if (exampleEntity == null) return null;

            return new ExampleEntity
            {
                Name = exampleEntity.Name,
                Age = exampleEntity.Age,
                GenderId = exampleEntity.GenderId,
            };
        }
    }
}
