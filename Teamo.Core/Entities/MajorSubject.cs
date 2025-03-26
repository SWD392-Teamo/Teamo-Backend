namespace Teamo.Core.Entities
{
    public class MajorSubject : BaseEntity
    {
        public int MajorId { get; set; }
        public Major Major { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}