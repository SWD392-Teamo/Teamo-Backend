using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
    public class Application : BaseEntity
    {
        public required int GroupId { get; set; }
        public Group Group { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public required int DestStudentId { get; set; }
        public User DestStudent { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public required int SrcStudentId { get; set; }
        public User SrcStudent { get; set; }
        public required DateTime RequestTime { get; set; } = DateTime.Now;
        [Column(TypeName = "nvarchar(1000)")]
        public required string RequestContent { get; set; }
        public required int GroupPositionId { get; set; }
        public GroupPosition GroupPosition { get; set; }
        [Column(TypeName = "varchar(50)")]
        public required ApplicationStatus Status { get; set; }
    }
}