using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces;

namespace Teamo.Core.Entities
{
	public class Major : BaseEntity, IDtoConvertible
	{
		public required string Code { get; set; }
		public required string Name { get; set; }
		public string? ImgUrl { get; set; }
        public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
		public List<Subject> Subjects { get; set; }
		public List<MajorSubject> MajorSubjects { get; set; }
		public MajorStatus Status { get; set; } = MajorStatus.Active;
    }
}