

namespace Teamo.Core.Entities
{
    public class Major : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
