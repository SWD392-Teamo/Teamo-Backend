using System.ComponentModel.DataAnnotations.Schema;

namespace Teamo.Core.Entities
{
    public class Semester : BaseEntity
    {
        [Column(TypeName = "nvarchar(1-0)")]
        public required string Name { get; set; }
        [Column(TypeName = "varchar(20)")]
        public required string Code { get; set; }
        [Column(TypeName = "Date")]
        public required DateOnly StartDate { get; set; }
        [Column(TypeName = "Date")]
        public required DateOnly EndDate { get; set; }
    }
}