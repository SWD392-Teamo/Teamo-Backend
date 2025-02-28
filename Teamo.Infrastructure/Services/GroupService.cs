
using Teamo.Core.Entities;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications;
using Teamo.Core.Specifications.Groups;
using Teamo.Core.Enums;

namespace Teamo.Infrastructure.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        public GroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddGroupPosition(GroupPosition groupPosition)
        {
            _unitOfWork.Repository<GroupPosition>().Add(groupPosition);
            await _unitOfWork.Repository<GroupPosition>().SaveAllAsync();
        }

        public async Task AddMemberToGroup(GroupMember groupMember)
        {
            var spec = new GroupMemberSpecification(new GroupMemberParams
            {
                Studentd = groupMember.StudentId,
                GroupId = groupMember.GroupId
            });

            var existingMember = await _unitOfWork.Repository<GroupMember>().GetEntityWithSpec(spec);
            if (existingMember != null)
            {
                throw new InvalidOperationException("This student already exists in this group!");
            }

            foreach(var position in groupMember.GroupMemberPositions)
            {
                var groupPosition = await _unitOfWork.Repository<GroupPosition>().GetEntityWithSpec(new GroupPositionSpecification(
                new GroupPositionParams { GroupId = groupMember.GroupId, PositionId = position.GroupPositionId}));
                if (groupPosition == null)
                {
                    throw new InvalidOperationException($"Position with ID {position.GroupPositionId} does not exist in group with ID {groupMember.GroupId}.");
                }
                if(groupPosition.Status == GroupPositionStatus.Closed)
                {
                    throw new InvalidOperationException($"Position with ID {position.GroupPositionId} is closed.");

                }
            }
            
            groupMember.Role = GroupMemberRole.Member;
            _unitOfWork.Repository<GroupMember>().Add(groupMember);
            await _unitOfWork.Repository<GroupMember>().SaveAllAsync();

            // update status of group position
            foreach (var groupPositionId in groupMember.GroupMemberPositions.Select(mp => mp.GroupPositionId))
            {
                await UpdateGroupPositionStatus(groupPositionId);
            }
        }

        public async Task CreateGroupAsync(Group group, int createdUserId)
        {
            // create group
            group.CreatedById = createdUserId;
            _unitOfWork.Repository<Group>().Add(group);
            await _unitOfWork.Repository<Group>().SaveAllAsync();

            // Add student to GroupMember with role Leader
            var groupMember = new GroupMember
            {
                GroupId = group.Id,
                StudentId = createdUserId,
                Role = GroupMemberRole.Leader,  
            };
            _unitOfWork.Repository<GroupMember>().Add(groupMember);

            await _unitOfWork.Repository<Group>().SaveAllAsync();
        }

        public async Task DeleteGroupAsync(Group group)
        {
            group.Status = GroupStatus.Deleted;
            _unitOfWork.Repository<Group>().Update(group);
            await _unitOfWork.Repository<Group>().SaveAllAsync();
        }
        public async Task RemoveGroupPositionAsync(GroupPosition groupPosition)
        {
            var groupMemberPositions = await _unitOfWork.Repository<GroupMemberPosition>()
                .ListAsync(new GroupMemberPositionSpecification(null, groupPosition.Id));
            if(groupMemberPositions.Count() > 0)
            {
                throw new InvalidOperationException("You cannot remove this position because there are still members assigned to it.");
            }
            groupPosition.Status = GroupPositionStatus.Deleted;
            _unitOfWork.Repository<GroupPosition>().Update(groupPosition);
            await _unitOfWork.Repository<GroupPosition>().SaveAllAsync();
        }

        public async Task RemoveMemberFromGroup(GroupMember groupMember)
        {
            var affectedGroupPositionIds = (await _unitOfWork.Repository<GroupMemberPosition>()
                                            .ListAsync(new GroupMemberPositionSpecification(groupMemberId: groupMember.Id)))
                                            .Select(mp => mp.GroupPositionId);

            _unitOfWork.Repository<GroupMember>().Delete(groupMember);
            await _unitOfWork.Repository<GroupMember>().SaveAllAsync();

            // update status of group position
            foreach (var groupPositionId in affectedGroupPositionIds)
            {
                await UpdateGroupPositionStatus(groupPositionId);
            }
        }

        public async Task<Group> GetGroupByIdAsync(int id)
        {
            var spec = new GroupSpecification(id);
            return await _unitOfWork.Repository<Group>().GetEntityWithSpec(spec);
        }

        public async Task<GroupMember> GetGroupMemberAsync(int groupId, int studentId)
        {
            var spec = new GroupMemberSpecification(new GroupMemberParams
            {
                GroupId = groupId,
                Studentd = studentId
            });
            return await _unitOfWork.Repository<GroupMember>().GetEntityWithSpec(spec);
        }

        public async Task<GroupPosition> GetGroupPositionAsync(int positionId)
        {
            var spec = new GroupPositionSpecification(positionId);
            return await _unitOfWork.Repository<GroupPosition>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Group>> GetGroupsAsync(ISpecification<Group> spec)
        {
            return await _unitOfWork.Repository<Group>().ListAsync(spec);
        }

        public async Task UpdateGroupAsync(Group group)
        {
            _unitOfWork.Repository<Group>().Update(group);
            await _unitOfWork.Repository<Group>().SaveAllAsync();
        }

        public async Task UpdateGroupMemberAsync(GroupMember groupMember)
        {
            var affectedGroupPositionIds = groupMember.GroupMemberPositions
                .Select(mp => mp.GroupPositionId)
                .Concat((await _unitOfWork.Repository<GroupMemberPosition>()
                    .ListAsync(new GroupMemberPositionSpecification(groupMemberId: groupMember.Id)))
                    .Select(mp => mp.GroupPositionId))
                .Distinct()
                .ToList();
            
            // update Group Member
            _unitOfWork.Repository<GroupMember>().Update(groupMember);
            await _unitOfWork.Repository<GroupMember>().SaveAllAsync(); 
            
            // update status of group position
            foreach(var groupPositionId in affectedGroupPositionIds)
            {
                await UpdateGroupPositionStatus(groupPositionId);
            }  
        }

        public async Task UpdateGroupPositionAsync(GroupPosition groupPosition)
        {
            // update GroupPosition
            _unitOfWork.Repository<GroupPosition>().Update(groupPosition);
            await _unitOfWork.Repository<GroupPosition>().SaveAllAsync();
        }
        /// <summary>
        /// Updates the status of a GroupPosition based on the current number of assigned members.
        /// If the number of assigned members reaches the maximum allowed, the status is set to "Closed".
        /// Otherwise, the status remains "Open".
        /// </summary>
        /// <param name="positionId">The ID of the GroupPosition to update.</param>
        private async Task UpdateGroupPositionStatus(int positionId)
        {

            var assignedMemberPositions = await _unitOfWork.Repository<GroupMemberPosition>()
                .ListAsync(new GroupMemberPositionSpecification(groupPositionId: positionId));
            var totalAssignedMembers = assignedMemberPositions.Count();

            var groupPosition = await _unitOfWork.Repository<GroupPosition>().GetByIdAsync(positionId);
            var maxAllowedMembers = groupPosition.Count;

            groupPosition.Status = totalAssignedMembers >= maxAllowedMembers
                                    ? GroupPositionStatus.Closed
                                    : GroupPositionStatus.Open;

            await UpdateGroupPositionAsync(groupPosition);
        }
    }
}
