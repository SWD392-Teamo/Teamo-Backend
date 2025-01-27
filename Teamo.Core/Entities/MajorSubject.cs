namespace Teamo.Core.Entities
{
    public class MajorSubject : BaseEntity
    {
        public required int MajorId { get; set; }
        public Major Major { get; set; }
        public required int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}