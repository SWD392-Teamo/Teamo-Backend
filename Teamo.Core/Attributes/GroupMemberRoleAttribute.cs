using System.ComponentModel.DataAnnotations;
using Teamo.Core.Entities;

namespace Teamo.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property |
                    AttributeTargets.Field, AllowMultiple = false)]
    public class GroupMemberRoleAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value == null) return false;
            var groupMemberRole = value.ToString();
            return groupMemberRole == GroupMemberRole.Member.ToString()
                || groupMemberRole == GroupMemberRole.Leader.ToString();
        }
    }
}