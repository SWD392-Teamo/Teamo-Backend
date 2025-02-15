using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Interfaces;

namespace Teamo.Core.Entities
{
	public class Major : BaseEntity, IDtoConvertible
	{
		public required string Code { get; set; }
		public required string Name { get; set; }
        public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
		public List<Subject> Subjects { get; set; }
		public List<MajorSubject> MajorSubjects { get; set; }
    }
}