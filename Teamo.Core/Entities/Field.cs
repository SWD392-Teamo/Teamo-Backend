using System.ComponentModel.DataAnnotations.Schema;

namespace Teamo.Core.Entities
{
    public class Field : BaseEntity
    {
        public required string Name { get; set; }
        public string Description { get; set; }
    }
}