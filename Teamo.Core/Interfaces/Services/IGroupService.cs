
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
        Task<IReadOnlyList<GroupMember>> GetAllGroupMembersAsync(int groupId);
        Task<bool> CreateGroupAsync(Group group, int createdUserId);
        Task<bool> AddMemberToGroup(GroupMember groupMember);
        Task<bool> AddGroupPosition(GroupPosition groupPosition);
        Task<bool> UpdateGroupAsync(Group group);
        Task<bool> UpdateGroupPositionAsync(GroupPosition groupPosition);
        Task<bool> UpdateGroupMemberAsync(GroupMember groupMember);
        Task<bool> DeleteGroupAsync(Group group);           
        Task<bool> RemoveGroupPositionAsync(GroupPosition groupPosition);      
        Task<bool> RemoveMemberFromGroup(GroupMember groupMember);
        Task<bool> CheckGroupLeaderAsync(int groupId, int studentId);
    }
}
