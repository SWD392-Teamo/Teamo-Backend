using System.ComponentModel.DataAnnotations.Schema;

namespace Teamo.Core.Entities
{
    public class Skill : BaseEntity
    {
        [Column(TypeName = "nvarchar(100)")]
        public required string Name { get; set; }
        [Column(TypeName = "varchar(100)")]
        public required string Type { get; set; }
    }
}