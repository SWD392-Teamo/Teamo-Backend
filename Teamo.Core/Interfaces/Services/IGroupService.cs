
using Teamo.Core.Entities;
using Teamo.Core.Specifications;

namespace Teamo.Core.Interfaces.Services
{
    public interface IGroupService
    {
        Task<IReadOnlyList<Group>> GetGroupsAsync(ISpecification<Group> spec);
        Task<Group> GetGroupByIdAsync(int id);
        Task<GroupPosition> GetGroupPositionAsync(int positionId);
        Task<GroupMember> GetGroupMemberAsync(int groupId, int studentId);
        Task CreateGroupAsync(Group group, int createdUserId);
        Task AddMemberToGroup(GroupMember groupMember);
        Task AddGroupPosition(GroupPosition groupPosition);
        Task UpdateGroupAsync(Group group);
        Task UpdateGroupPositionAsync(GroupPosition groupPosition);
        Task UpdateGroupMemberAsync(GroupMember groupMember);
        Task DeleteGroupAsync(Group group);           
        Task RemoveGroupPositionAsync(GroupPosition groupPosition);      
        Task RemoveMemberFromGroup(GroupMember groupMember);
        
        
    }
}
