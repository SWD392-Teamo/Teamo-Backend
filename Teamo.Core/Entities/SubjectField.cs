namespace Teamo.Core.Entities
{
    public class SubjectField : BaseEntity
    {
        public required int FieldId { get; set; }
        public Field Field { get; set; }
        public required int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}