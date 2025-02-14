using Teamo.Core.Entities;
using Teamo.Core.Specifications.Subjects;

namespace Teamo.Core.Interfaces.Services
{
    public interface ISubjectService
    {
        Task<Subject> GetSubjectByIdAsync(int id);
        Task<IReadOnlyList<Subject>> GetSubjectsAsync(SubjectParams subjectParams);
        Task<bool> CreateSubjectAsync(Subject subject);
        Task<bool> UpdateSubjectAsync (Subject subject);
        Task<int> CountSubjectsAsync(SubjectParams subjectParams);
    }
}