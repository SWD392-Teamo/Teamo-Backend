
using Teamo.Core.Entities;
using Teamo.Core.Specifications;

namespace Teamo.Core.Interfaces.Services
{
    public interface IGroupService
    {
        Task<IReadOnlyList<Group>> GetGroupsAsync(ISpecification<Group> spec);
        Task<Group> GetGroupByIdAsync(ISpecification<Group> spec);
        Task<Group> UpdateGroupAsync(Group group);  
        Task<Group> CreateGroupAsync(Group group);
    }
}
