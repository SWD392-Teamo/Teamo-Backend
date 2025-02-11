using System.ComponentModel.DataAnnotations.Schema;
using Teamo.Core.Enums;

namespace Teamo.Core.Entities
{
    public class Student
    {
        public required int Id { get; set; }
        public required string Code { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required Gender Gender { get; set; } 
        public string Phone { get; set; }
        public string ImgUrl { get; set; }
        public required int MajorId { get; set; }
    }
}