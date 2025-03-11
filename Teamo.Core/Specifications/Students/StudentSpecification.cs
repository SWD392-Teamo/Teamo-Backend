using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Students
{
    public class StudentSpecification : BaseSpecification<Student>
    {
        public StudentSpecification(string email) : base(x => x.Email.Equals(email))
        {
        }
    }
}
