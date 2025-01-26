using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Entities.Identity;

namespace Teamo.Core.Entities
{
    public class Link : BaseEntity
    {
        public required string Name { get; set; }
        public required string Url { get; set; }
        public required int StudentId { get; set; }
        public User Student { get; set; }
    }
}