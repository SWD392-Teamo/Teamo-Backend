using System.ComponentModel.DataAnnotations.Schema;

namespace Teamo.Core.Entities
{
	public class Major : BaseEntity
	{
		public required string Code { get; set; }
		public required string Name { get; set; }
        public required DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
		public List<Subject> Subjects { get; set; }
		public List<MajorSubject> MajorSubjects { get; set; }
    }
}