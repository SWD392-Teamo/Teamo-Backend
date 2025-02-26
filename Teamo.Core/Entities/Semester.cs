using Teamo.Core.Enums;
using Teamo.Core.Interfaces;

namespace Teamo.Core.Entities
{
    public class Semester : BaseEntity, IDtoConvertible
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public SemesterStatus Status { get; set; }
    }
}