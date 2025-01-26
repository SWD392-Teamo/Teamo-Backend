using System.ComponentModel.DataAnnotations.Schema;

namespace Teamo.Core.Entities
{
    public class Field : BaseEntity
    {
        [Column(TypeName = "nvarchar(100)")]
        public required string Name { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string? Description { get; set; }
    }
}