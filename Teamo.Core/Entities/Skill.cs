using System.ComponentModel.DataAnnotations.Schema;

namespace Teamo.Core.Entities
{
    public class Skill : BaseEntity
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
    }
}