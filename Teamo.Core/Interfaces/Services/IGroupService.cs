
using Teamo.Core.Entities;
using Teamo.Core.Specifications;

namespace Teamo.Core.Interfaces.Services
{
    public interface IGroupService
    {
        Task<IReadOnlyList<Group>> GetGroupsAsync(ISpecification<Group> spec);
        Task<Group> GetGroupByIdAsync(ISpecification<Group> spec);
        Task<bool> UpdateGroupAsync(Group group);  
        Task<bool> CreateGroupAsync(Group group, int createdUserId);
    }
}
