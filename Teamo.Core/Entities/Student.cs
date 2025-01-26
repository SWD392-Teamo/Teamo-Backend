using System.ComponentModel.DataAnnotations.Schema;

namespace Teamo.Core.Entities
{
    public class Student
    {
        [Column(TypeName = "nvarchar(450)")]
        public required string Id { get; set; }
        [Column(TypeName = "varchar(20)")]
        public required string Code { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public required string FirstName { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public required string LastName { get; set; }
        [Column(TypeName = "varchar(50)")]
        public required string Username { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public required string Email { get; set; }
        [Column(TypeName = "varchar(50)")]
        public required string Gender { get; set; } 
        [Column(TypeName = "varchar(20)")]
        public string? Phone { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string? ImgUrl { get; set; }
        public required int MajorId { get; set; }
        public Major Major { get; set; }
    }
}