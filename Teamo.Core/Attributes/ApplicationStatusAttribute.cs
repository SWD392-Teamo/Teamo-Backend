using System.ComponentModel.DataAnnotations;
using Teamo.Core.Entities;

namespace Teamo.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property |
                    AttributeTargets.Field, AllowMultiple = false)]
    public class ApplicationStatusAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value == null) return false;
            var applicationStatus = value.ToString();
            return applicationStatus == ApplicationStatus.Requested.ToString()
                || applicationStatus == ApplicationStatus.Accepted.ToString()
                || applicationStatus == ApplicationStatus.Declined.ToString();
        }
    }
}