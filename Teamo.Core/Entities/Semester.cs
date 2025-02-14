using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
    public class Semester : BaseEntity
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public SemesterStatus Status { get; set; }
    }
}