using System.ComponentModel.DataAnnotations.Schema;

namespace Teamo.Core.Entities
{
    public class Subject : BaseEntity
    {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public required DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public List<Field> Fields { get; set; }
        public List<Major> Majors { get; set;}
        public List<MajorSubject> MajorSubjects { get; set;}
    }
}