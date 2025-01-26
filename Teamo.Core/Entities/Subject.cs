using System.ComponentModel.DataAnnotations.Schema;

namespace Teamo.Core.Entities
{
    public class Subject : BaseEntity
    {
        [Column(TypeName = "varchar(20)")]
        public required string Code { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public required string Name { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string? Description { get; set; }
        [Column(TypeName = "Date")]
        public required DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}