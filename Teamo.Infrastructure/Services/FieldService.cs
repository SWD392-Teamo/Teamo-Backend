using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications;
using Teamo.Core.Specifications.Fields;
using Teamo.Core.Specifications.SubjectFields;

namespace Teamo.Infrastructure.Services
{
    public class FieldService : IFieldService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FieldService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<Field>> GetFieldsWithSpecAsync(FieldParams fieldParams)
        {
            // Get all fields
            var fields = await _unitOfWork.Repository<Field>().ListAllAsync();

            // Filter by SubjectId if it has value
            if (fieldParams.SubjectId.HasValue)
            {
                var spec = new SubjectFieldSpecification((int) fieldParams.SubjectId);
                var subjectFields = await _unitOfWork.Repository<SubjectField>().ListAsync(spec);
                var allSubjectFields = subjectFields.Select(s => s.Field).ToList();
                fields = fields.Where(allSubjectFields.Contains).ToList();
            }  

            // Apply filter, paging, search
            var fieldSpec = new FieldSpecification(fieldParams);
            fields = SpecificationEvaluator<Field>.GetQuery(fields.AsQueryable(), fieldSpec).ToList();

            return fields;          
        }

        public async Task<Field> GetFieldByIdAsync(int id)
        {
            var fieldSpec = new FieldSpecification(id);
            var field = await _unitOfWork.Repository<Field>().GetEntityWithSpec(fieldSpec);

            return field;
        }

        public async Task<bool> CreateFieldAsync(Field field)
        {
            _unitOfWork.Repository<Field>().Add(field);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> DeleteFieldAsync(Field field)
        {
            //Delete relevant entries in SubjectField table
            var subjectFieldSpec = new SubjectFieldSpecification(field.Id);
            var subjectFields = await _unitOfWork.Repository<SubjectField>().ListAsync(subjectFieldSpec);
            _unitOfWork.Repository<SubjectField>().DeleteRange(subjectFields);
            
            //Delete field
            _unitOfWork.Repository<Field>().Delete(field);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> CheckDuplicateNameField(string name)
        {
            var result = true;

            var existFieldSpec = new FieldSpecification(name);
            var existField = await _unitOfWork.Repository<Field>().GetEntityWithSpec(existFieldSpec);

            if(existField != null) result = false;

            return result;
        }

        public async Task<int> CountAsync(FieldParams fieldParams)
        {
            // Get all fields
            var fields = await _unitOfWork.Repository<Field>().ListAllAsync();

            // Filter by SubjectId if it has value
            if (fieldParams.SubjectId.HasValue)
            {
                var spec = new SubjectFieldSpecification((int)fieldParams.SubjectId);
                var subjectFields = await _unitOfWork.Repository<SubjectField>().ListAsync(spec);
                var allSubjectFields = subjectFields.Select(s => s.Field).ToList();
                fields = fields.Where(allSubjectFields.Contains).ToList();
            }

            // Apply filter and count
            var countSpec = new FieldCountSpecification(fieldParams);
            var count = SpecificationEvaluator<Field>
                .GetQuery(fields.AsQueryable(), countSpec)
                .Count();

            return count;
        }
    }
}