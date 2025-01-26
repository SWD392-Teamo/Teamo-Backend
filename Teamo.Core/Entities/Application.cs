using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Attributes;

namespace Teamo.Core.Entities
{
    public class Application : BaseEntity
    {
        public required int GroupId { get; set; }
        public Group Group { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public required string DestStudentId { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public required string SrcStudentId { get; set; }
        public required DateTime RequestTime { get; set; } = DateTime.Now;
        [Column(TypeName = "nvarchar(1000)")]
        public required string RequestContent { get; set; }
        public required int GroupPositionId { get; set; }
        public GroupPosition GroupPosition { get; set; }
        [Column(TypeName = "varchar(50)")]
        public required string Status { get; set; } = ApplicationStatus.Requested.ToString();
    }
}