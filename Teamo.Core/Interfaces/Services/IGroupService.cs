
using Teamo.Core.Entities;
using Teamo.Core.Specifications;

namespace Teamo.Core.Interfaces.Services
{
    public interface IGroupService
    {
        Task<IReadOnlyList<Group>> GetGroupsAsync(ISpecification<Group> spec);
        Task<int> CountGroupsAsync(ISpecification<Group> spec);
        Task<Group> GetGroupByIdAsync(int id);
        Task<GroupPosition> GetGroupPositionAsync(int positionId);
        Task<GroupMember> GetGroupMemberAsync(int groupId, int studentId);
        Task<IReadOnlyList<GroupMember>> GetAllGroupMembersAsync(int groupId);
        Task CreateGroupAsync(Group group, int createdUserId);
        Task AddMemberToGroup(GroupMember groupMember);
        Task AddGroupPosition(GroupPosition groupPosition);
        Task<bool> UpdateGroupAsync(Group group);
        Task UpdateGroupPositionAsync(GroupPosition groupPosition);
        Task UpdateGroupMemberAsync(GroupMember groupMember);
        Task DeleteGroupAsync(Group group);           
        Task RemoveGroupPositionAsync(GroupPosition groupPosition);      
        Task RemoveMemberFromGroup(GroupMember groupMember);
        Task<bool> CheckGroupLeaderAsync(int groupId, int studentId);
    }
}
