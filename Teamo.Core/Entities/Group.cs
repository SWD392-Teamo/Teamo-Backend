using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
	public class Group : BaseEntity
	{
		public required string Name { get; set; }
		public required string Title { get; set; } 
		public string ImgUrl { get; set; }
        public required int SemesterId { get; set; }
		public Semester Semester { get; set; }
		public string Description { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public int CreatedById { get; set; } 
		public User CreatedByUser { get; set; }
		public required int MaxMember { get; set; }
		public GroupStatus Status { get; set; } = GroupStatus.Recruiting;
		public required int FieldId { get; set; }
		public Field Field { get; set; }
		public required int SubjectId { get; set; }
		public Subject Subject { get; set; }
		public List<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
		public List<GroupPosition> GroupPositions { get; set; } = new List<GroupPosition>();	
		public List<Application> Applications { get; set; } = new List<Application>();
		public List<Post> Posts { get; set; } = new List<Post>();
	}
}