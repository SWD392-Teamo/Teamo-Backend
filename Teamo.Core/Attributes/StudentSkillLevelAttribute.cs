using System.ComponentModel.DataAnnotations;
using Teamo.Core.Entities;

namespace Teamo.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property |
                    AttributeTargets.Field, AllowMultiple = false)]
    public class StudentSkillLevelAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value == null) return false;
            var studentSkillLevel = value.ToString();
            return studentSkillLevel == StudentSkillLevel.Beginner.ToString()
                || studentSkillLevel == StudentSkillLevel.PreIntermediate.ToString()
                || studentSkillLevel == StudentSkillLevel.Intermediate.ToString()
                || studentSkillLevel == StudentSkillLevel.Advanced.ToString();
        }
    }
}