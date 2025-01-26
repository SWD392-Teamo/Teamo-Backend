using System.ComponentModel.DataAnnotations;
using Teamo.Core.Entities;

namespace Teamo.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property |
                    AttributeTargets.Field, AllowMultiple = false)]
    public class GenderAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value == null) return false;
            var gender = value.ToString();
            return gender == Gender.Male.ToString()
                || gender == Gender.Female.ToString()
                || gender == Gender.Other.ToString();
        }
    }
}