using Teamo.Core.Interfaces;

namespace Teamo.Core.Entities
{
    public class ExampleEntity : BaseEntity, IDtoConvertible
    {
        public required string Name { get; set; }
        public required int Age { get; set; }
        public required int GenderId { get; set; }
        public ExampleGender Gender { get; set; }
    }
}
