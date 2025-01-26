using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Teamo.Core.Entities.Identity
{
    public enum UserRole
    {
        [EnumMember(Value = "Student")]
        Student,
        [EnumMember(Value = "Admin")]
        Admin
    }
}
