using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces;

namespace Teamo.Core.Entities
{
    public class Application : BaseEntity, IDtoConvertible
    {
        public required int GroupId { get; set; }
        public Group Group { get; set; }
        public required int DestStudentId { get; set; }
        public User DestStudent { get; set; }
        public required int SrcStudentId { get; set; }
        public User SrcStudent { get; set; }
        public required DateTime RequestTime { get; set; } = DateTime.Now;
        public required string RequestContent { get; set; }
        public required int GroupPositionId { get; set; }
        public GroupPosition GroupPosition { get; set; }
        public required ApplicationStatus Status { get; set; }
    }
}