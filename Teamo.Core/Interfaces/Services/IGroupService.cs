
using Teamo.Core.Entities;
using Teamo.Core.Specifications;

namespace Teamo.Core.Interfaces.Services
{
    public interface IGroupService
    {
        Task<IReadOnlyList<Group>> GetGroupsAsync(ISpecification<Group> spec);
        Task<Group> GetGroupByIdAsync(int id);
        Task UpdateGroupAsync(Group group);  
        Task CreateGroupAsync(Group group, int createdUserId, IEnumerable<GroupPosition> groupPositions);
        Task DeleteGroupAsync(Group group);
        Task<IReadOnlyList<Group>> GetGroupsByMemberIdAsync(ISpecification<GroupMember> spec);
        Task AddMemberToGroup(GroupMember groupMember);
    }
}
