using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
	public class Group : BaseEntity
	{
		public required string Name { get; set; }
		public required string Title { get; set; }
		public required int SemesterId { get; set; }
		public Semester Semester { get; set; }
		public string Description { get; set; }
		public required DateTime CreatedAt { get; set; } = DateTime.Now;
		public required int CreatedById { get; set; }
		public User CreatedByUser { get; set; }
		public required int MaxMember { get; set; }
		public required GroupStatus Status { get; set; }
		public required int FieldId { get; set; }
		public Field Field { get; set; }
		public required int SubjectId { get; set; }
		public Subject Subject { get; set; }
		public List<GroupMember> GroupMembers { get; set; }
		public List<GroupPosition> GroupPositions { get; set; }
		public List<Application> Applications { get; set; }
	}
}