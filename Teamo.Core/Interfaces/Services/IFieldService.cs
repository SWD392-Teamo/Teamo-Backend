using Teamo.Core.Entities;
using Teamo.Core.Specifications.Fields;

namespace Teamo.Core.Interfaces.Services
{
    public interface IFieldService
    {
        Task<IReadOnlyList<Field>> GetFieldsWithSpecAsync(FieldParams fieldParams);
        Task<Field> GetFieldByIdAsync(int id);
        Task<Field> CreateFieldAsync(Field field);
        Task<bool> DeleteFieldAsync(Field field);
        Task<Field> UpdateFieldAsync(Field field);
        Task<bool> CheckDuplicateNameField(string name);  
        Task<int> CountAsync(FieldParams fieldParams);
    }
}