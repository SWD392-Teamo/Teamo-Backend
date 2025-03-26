using System.CodeDom;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications;
using Teamo.Core.Specifications.Fields;
using Teamo.Core.Specifications.Groups;
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

        public async Task<IReadOnlyList<Field>> GetFieldsAsync(FieldParams fieldParams)
        {
            //Get fields by search param
            var fieldSpec = new FieldSpecification(fieldParams);
            var fields = await _unitOfWork.Repository<Field>().ListAsync(fieldSpec);

            if(!fieldParams.SubjectId.HasValue)
            {
                return fields;
            }
            
            // Filter by SubjectId if it has value
            var subjectFieldParams = new SubjectFieldParams{ SubjectId = fieldParams.SubjectId };
            var spec = new SubjectFieldSpecification(subjectFieldParams);
            var subjectFields = await _unitOfWork.Repository<SubjectField>().ListAsync(spec);
            var filteredFields = subjectFields.Select(s => s.Field).ToList();
            fields = fields.Where(filteredFields.Contains).ToList();

            return fields;          
        }

        public async Task<int> CountFieldsAsync(FieldParams fieldParams)
        {
            var fieldSpec = new FieldSpecification(fieldParams);
            var count = await _unitOfWork.Repository<Field>().CountAsync(fieldSpec);
            return count;
        }

        public async Task<Field> GetFieldByIdAsync(int id)
        {
            var fieldSpec = new FieldSpecification(id);
            var field = await _unitOfWork.Repository<Field>().GetEntityWithSpec(fieldSpec);

            return field;
        }

        public async Task<Field> CreateFieldAsync(Field field)
        {
            _unitOfWork.Repository<Field>().Add(field);
            await _unitOfWork.Complete();
            return await GetFieldByIdAsync(field.Id);
        }

        public async Task<bool> DeleteFieldAsync(Field field)
        {
            //Check for existing groups in specified field
            var groupSpec = new GroupByFieldIdSpecification(field.Id);
            var group = await _unitOfWork.Repository<Group>().GetEntityWithSpec(groupSpec);
            if(group != null) throw new ArgumentException("This field cannot be deleted because it is associated with existing groups..");

            //Delete relevant entries in SubjectField table
            var subjectFieldSpec = new SubjectFieldSpecification(field.Id);
            var subjectFields = await _unitOfWork.Repository<SubjectField>().ListAsync(subjectFieldSpec);
            _unitOfWork.Repository<SubjectField>().DeleteRange(subjectFields);
            
            //Delete field
            _unitOfWork.Repository<Field>().Delete(field);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> UpdateFieldAsync(Field field)
        {
            _unitOfWork.Repository<Field>().Update(field);
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
    }
}