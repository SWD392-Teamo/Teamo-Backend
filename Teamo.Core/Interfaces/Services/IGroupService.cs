﻿
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
        Task AddGroupPosition(GroupPosition groupPosition);
        Task UpdateGroupPositionAsync(GroupPosition groupPosition, IEnumerable<int> skillIds);
        Task<GroupPosition> GetGroupPositionByIdAsync(int id);
        Task RemoveMemberFromGroup(GroupMember groupMember);
        Task<GroupMember> GetGroupMemberByIdAsync(int groupMemberId);
    }
}
