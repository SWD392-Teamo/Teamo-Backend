using System.ComponentModel.DataAnnotations.Schema;

namespace Teamo.Core.Entities
{
    public class Link : BaseEntity
    {
        [Column(TypeName = "nvarchar(100)")]
        public required string Name { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public required string Url { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public required string StudentId { get; set; }
    }
}