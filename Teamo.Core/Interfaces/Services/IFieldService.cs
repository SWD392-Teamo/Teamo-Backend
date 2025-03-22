using Teamo.Core.Entities;
using Teamo.Core.Specifications.Fields;

namespace Teamo.Core.Interfaces.Services
{
    public interface IFieldService
    {
        Task<IReadOnlyList<Field>> GetFieldsAsync(FieldParams fieldParams);
        Task<int> CountFieldsAsync(FieldParams fieldParams);
        Task<Field> GetFieldByIdAsync(int id);
        Task<Field> CreateFieldAsync(Field field);
        Task<bool> DeleteFieldAsync(Field field);
        Task<bool> UpdateFieldAsync(Field field);
        Task<bool> CheckDuplicateNameField(string name);  
    }
}