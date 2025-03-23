
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

        public async Task<bool> AddGroupPosition(GroupPosition groupPosition)
        {
            _unitOfWork.Repository<GroupPosition>().Add(groupPosition);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> AddMemberToGroup(GroupMember groupMember)
        {
            var result = true;
            
            var spec = new GroupMemberSpecification(new GroupMemberParams
            {
                StudentId = groupMember.StudentId,
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
            result = await _unitOfWork.Repository<GroupMember>().SaveAllAsync();

            // update status of group position
            foreach (var groupPositionId in groupMember.GroupMemberPositions.Select(mp => mp.GroupPositionId))
            {
                result = await UpdateGroupPositionStatus(groupPositionId);
                if(!result) throw new Exception($"Position with ID {groupPositionId} failed to update status.");
            }
            return result;
        }

        public async Task<bool> CreateGroupAsync(Group group, int createdUserId)
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

            return await _unitOfWork.Complete();
        }

        public async Task<bool> DeleteGroupAsync(Group group)
        {
            group.Status = GroupStatus.Deleted;
            _unitOfWork.Repository<Group>().Update(group);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> RemoveGroupPositionAsync(GroupPosition groupPosition)
        {
            var groupMemberPositions = await _unitOfWork.Repository<GroupMemberPosition>()
                .ListAsync(new GroupMemberPositionSpecification(groupPositionId: groupPosition.Id));
            if(groupMemberPositions.Count() > 0)
            {
                throw new InvalidOperationException("You cannot remove this position because there are still members assigned to it.");
            }
            groupPosition.Status = GroupPositionStatus.Deleted;
            _unitOfWork.Repository<GroupPosition>().Update(groupPosition);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> RemoveMemberFromGroup(GroupMember groupMember)
        {
            var result = true;
            var affectedGroupPositionIds = (await _unitOfWork.Repository<GroupMemberPosition>()
                .ListAsync(new GroupMemberPositionSpecification(groupMemberId: groupMember.Id)))
                .Select(mp => mp.GroupPositionId);

            _unitOfWork.Repository<GroupMember>().Delete(groupMember);
            result = await _unitOfWork.Repository<GroupMember>().SaveAllAsync();

            // update status of group position
            foreach (var groupPositionId in affectedGroupPositionIds)
            {
                result = await UpdateGroupPositionStatus(groupPositionId);
                if(!result) throw new Exception($"Position with ID {groupPositionId} failed to update status.");
            }
            return result;
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
                StudentId = studentId
            });
            return await _unitOfWork.Repository<GroupMember>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<GroupMember>> GetAllGroupMembersAsync(int groupId)
        {
            var spec = new GroupMemberSpecification(new GroupMemberParams
            {
                GroupId = groupId
            });
            return await _unitOfWork.Repository<GroupMember>().ListAsync(spec);
        }

        public async Task<GroupPosition> GetGroupPositionAsync(int positionId)
        {
            var spec = new GroupPositionSpecification(positionId);
            return await _unitOfWork.Repository<GroupPosition>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Group>> GetGroupsAsync(ISpecification<Group> spec)
        {
            await UpdateGroupStatus();
            return await _unitOfWork.Repository<Group>().ListAsync(spec);
        }

        public async Task<bool> UpdateGroupAsync(Group group)
        {
            _unitOfWork.Repository<Group>().Update(group);
            var result = await _unitOfWork.Repository<Group>().SaveAllAsync();
            return result;
        }

        public async Task<bool> UpdateGroupMemberAsync(GroupMember groupMember)
        {
            var result = true;
            var affectedGroupPositionIds = groupMember.GroupMemberPositions
                .Select(mp => mp.GroupPositionId)
                .Concat((await _unitOfWork.Repository<GroupMemberPosition>()
                    .ListAsync(new GroupMemberPositionSpecification(groupMemberId: groupMember.Id)))
                    .Select(mp => mp.GroupPositionId))
                .Distinct()
                .ToList();
            
            // update Group Member
            _unitOfWork.Repository<GroupMember>().Update(groupMember);
            result = await _unitOfWork.Repository<GroupMember>().SaveAllAsync(); 
            
            // update status of group position
            foreach(var groupPositionId in affectedGroupPositionIds)
            {
                result = await UpdateGroupPositionStatus(groupPositionId);
                if(!result) throw new Exception($"Position with ID {groupPositionId} failed to update status.");
            } 

            return result; 
        }

        public async Task<bool> UpdateGroupPositionAsync(GroupPosition groupPosition)
        {
            // update GroupPosition
            _unitOfWork.Repository<GroupPosition>().Update(groupPosition);
            return await _unitOfWork.Repository<GroupPosition>().SaveAllAsync();
        }

        /// <summary>
        /// Updates the status of a GroupPosition based on the current number of assigned members.
        /// If the number of assigned members reaches the maximum allowed, the status is set to "Closed".
        /// Otherwise, the status remains "Open".
        /// </summary>
        /// <param name="positionId">The ID of the GroupPosition to update.</param>
        private async Task<bool> UpdateGroupPositionStatus(int positionId)
        {

            var assignedMemberPositions = await _unitOfWork.Repository<GroupMemberPosition>()
                .ListAsync(new GroupMemberPositionSpecification(groupPositionId: positionId));
            var totalAssignedMembers = assignedMemberPositions.Count();

            var groupPosition = await _unitOfWork.Repository<GroupPosition>().GetByIdAsync(positionId);
            var maxAllowedMembers = groupPosition.Count;

            groupPosition.Status = totalAssignedMembers >= maxAllowedMembers
                                    ? GroupPositionStatus.Closed
                                    : GroupPositionStatus.Open;

            return await UpdateGroupPositionAsync(groupPosition);
        }

        public async Task<bool> CheckGroupLeaderAsync(int groupId, int studentId)
        {
            var groupMember = await GetGroupMemberAsync(groupId, studentId);
            return groupMember != null && groupMember.Role == GroupMemberRole.Leader;
        }

        //Update group status to Archived if semester is over
        private async Task UpdateGroupStatus()
        {
            var spec = new GroupSpecification();
            var groups = await _unitOfWork.Repository<Group>().ListAsync(spec);
            foreach (var g in groups)
            {
                if(g.Semester.Status == SemesterStatus.Past && g.Status != GroupStatus.Archived)
                {
                    g.Status = GroupStatus.Archived;
                    _unitOfWork.Repository<Group>().Update(g);
                }
            }

            await _unitOfWork.Complete();
        }
    }
}
