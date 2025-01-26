using System.ComponentModel.DataAnnotations;
using Teamo.Core.Entities;

namespace Teamo.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property |
                    AttributeTargets.Field, AllowMultiple = false)]
    public class GroupPositionStatusAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value == null) return false;
            var groupPositionStatus = value.ToString();
            return groupPositionStatus == GroupPositionStatus.Open.ToString()
                || groupPositionStatus == GroupPositionStatus.Closed.ToString();
        }
    }
}