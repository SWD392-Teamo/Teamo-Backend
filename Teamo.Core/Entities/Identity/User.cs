using Microsoft.AspNetCore.Identity;
using System.Runtime.Serialization;
using Teamo.Core.Enums;

namespace Teamo.Core.Entities.Identity
{
    public class User : IdentityUser<int>
    {
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public DateOnly Dob { get; set; }
        public string Phone { get; set; }
        public string ImgUrl { get; set; }
        public UserStatus Status { get; set; }
        public string Description { get; set; }
        public int MajorID { get; set; }
        public Major Major {  get; set; }
        public List<Link> Links { get; set; }
        public List<Skill> Skills { get; set; }
        public List<StudentSkill> StudentSkills { get; set; }
    }
}
