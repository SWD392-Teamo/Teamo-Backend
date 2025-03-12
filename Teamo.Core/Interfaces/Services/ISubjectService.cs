using Teamo.Core.Entities;
using Teamo.Core.Specifications.Subjects;

namespace Teamo.Core.Interfaces.Services
{
    public interface ISubjectService
    {
        Task<Subject> GetSubjectByIdAsync(int id);
        Task<IReadOnlyList<Subject>> GetSubjectsAsync(SubjectParams subjectParams);
        Task<Subject> CreateSubjectAsync(Subject subject);
        Task<bool> UpdateSubjectAsync(Subject subject);
        Task<bool> DeleteSubjectAsync(Subject subject);
        Task<int> CountSubjectsAsync(SubjectParams subjectParams);
        Task<bool> CheckDuplicateCodeSubject(string code);
    }
}