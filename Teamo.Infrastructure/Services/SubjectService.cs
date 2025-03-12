using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications;
using Teamo.Core.Specifications.Majors;
using Teamo.Core.Specifications.MajorSubjects;
using Teamo.Core.Specifications.Subjects;

namespace Teamo.Infrastructure.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            var subjectSpec = new SubjectSpecification(id);
            return await _unitOfWork.Repository<Subject>().GetEntityWithSpec(subjectSpec);
        }

        public async Task<IReadOnlyList<Subject>> GetSubjectsAsync(SubjectParams subjectParams)
        {
            // Get all subject
            var subjects = await _unitOfWork.Repository<Subject>().ListAllAsync();

            // Filter by MajorId if it has value
            if (subjectParams.MajorId.HasValue)
            {
                var spec = new MajorSubjectSpecification((int) subjectParams.MajorId);
                var majorSubjects = await _unitOfWork.Repository<MajorSubject>().ListAsync(spec);
                var allMajorSubjects = majorSubjects.Select(s => s.Subject).ToList();
                subjects = subjects.Where(allMajorSubjects.Contains).ToList();
            }  

            // Apply filter, paging, search
            var subjectSpec = new SubjectSpecification(subjectParams);
            subjects = SpecificationEvaluator<Subject>.GetQuery(subjects.AsQueryable(), subjectSpec).ToList();

            return subjects;          
        }

        public async Task<Subject> CreateSubjectAsync(Subject subject)
        {
            _unitOfWork.Repository<Subject>().Add(subject);
            await _unitOfWork.Complete();
            return await GetSubjectByIdAsync(subject.Id);
        }

        public async Task<bool> UpdateSubjectAsync(Subject subject)
        {
            _unitOfWork.Repository<Subject>().Update(subject);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> DeleteSubjectAsync(Subject subject)
        {
            subject.Status = SubjectStatus.Inactive;
            _unitOfWork.Repository<Subject>().Update(subject);
            return await _unitOfWork.Complete();
        }

        public async Task<int> CountSubjectsAsync(SubjectParams subjectParams)
        {
            // Get all subject
            var subjects = await _unitOfWork.Repository<Subject>().ListAllAsync();

            // Filter by MajorId if it has value
            if (subjectParams.MajorId.HasValue)
            {
                var spec = new MajorSubjectSpecification((int)subjectParams.MajorId);
                var majorSubjects = await _unitOfWork.Repository<MajorSubject>().ListAsync(spec);
                var allMajorSubjects = majorSubjects.Select(s => s.Subject).ToList();
                subjects = subjects.Where(allMajorSubjects.Contains).ToList();
            }

            // Apply filter and count
            var countSpec = new SubjectCountSpecification(subjectParams);
            var count = SpecificationEvaluator<Subject>
                .GetQuery(subjects.AsQueryable(), countSpec)
                .Count();

            return count;
        }

        public async Task<bool> CheckDuplicateCodeSubject(string code)
        {
            var result = true;
            var subjectSpec = new SubjectSpecification(code);
            var duplicateCode = await _unitOfWork.Repository<Subject>().GetEntityWithSpec(subjectSpec);
            if(duplicateCode != null) result = false;
            return result;
        }
    }
}