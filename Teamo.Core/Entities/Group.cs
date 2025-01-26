using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Attributes;

namespace Teamo.Core.Entities
{
	public class Group : BaseEntity
	{
		[Column(TypeName = "nvarchar(100)")]
		public required string Name { get; set; }
		[Column(TypeName = "nvarchar(200)")]
		public required string Title { get; set; }
		public required int SemesterId { get; set; }
		public Semester Semester { get; set; }
		[Column(TypeName = "nvarchar(1000)")]
		public required string Description { get; set; }
		public required DateTime CreatedAt { get; set; } = DateTime.Now;
		[Column(TypeName = "nvarchar(450)")]
		public required string CreatedById { get; set; }
		public required int MaxMember { get; set; }
		[Column(TypeName = "varchar(50)")]
		public required string Status { get; set; } = GroupStatus.Recruiting.ToString();
		public required int FieldId { get; set; }
		public Field Field { get; set; }
		public required int SubjectId { get; set; }
		public Subject Subject { get; set; }
		public IReadOnlyList<GroupMember> GroupMembers { get; set; }
		public IReadOnlyList<GroupPosition> GroupPositions { get; set; }
		public IReadOnlyList<Application> Applications { get; set; }
	}
}