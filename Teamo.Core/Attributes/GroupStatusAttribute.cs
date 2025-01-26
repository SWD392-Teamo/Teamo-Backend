using System.ComponentModel.DataAnnotations;
using Teamo.Core.Entities;

namespace Teamo.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property |
                    AttributeTargets.Field, AllowMultiple = false)]
    public class GroupStatusAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value == null) return false;
            var groupStatus = value.ToString();
            return groupStatus == GroupStatus.Recruiting.ToString()
                || groupStatus == GroupStatus.Full.ToString()
                || groupStatus == GroupStatus.Archived.ToString();
        }
    }
}