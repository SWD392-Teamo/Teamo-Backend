
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

            //foreach (var memberPosition in groupMember.GroupMemberPositions)
            //{
            //    var memberPositions = await _unitOfWork.Repository<GroupMemberPosition>()
            //        .ListAsync(new GroupMemberPositionSpecification(memberPosition.GroupPositionId));
            //    var totalMemberPositions = memberPositions.Count();
            //    if (totalMemberPositions >= memberPosition.GroupPosition.Count)
            //    {
            //        memberPosition.GroupPosition.Status = GroupPositionStatus.Closed;
            //        _unitOfWork.Repository<GroupMemberPosition>().Update(memberPosition);
            //    }
            //}
            
            //await _unitOfWork.Repository<GroupMemberPosition>().SaveAllAsync();

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
            _unitOfWork.Repository<GroupMember>().Delete(groupMember);
            await _unitOfWork.Repository<GroupMember>().SaveAllAsync();
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

        public async Task UpdateGroupMemberAsync(GroupMember groupMember, IEnumerable<int> groupPositionIds)
        {
            var existingMemberPositions = groupMember.GroupMemberPositions;
            var existingMemberPositionIds = existingMemberPositions.Select(s => s.GroupPositionId).ToList();

            var positionsToAdd = groupPositionIds.Except(existingMemberPositionIds)
                              .Select(positionId => new GroupMemberPosition
                              {
                                  GroupMemberId = groupMember.Id,
                                  GroupPositionId = positionId
                              });
            var positionsToRemove = existingMemberPositions.Where(s => !groupPositionIds.Contains(s.GroupPositionId));

            // update Group Member
            _unitOfWork.Repository<GroupMember>().Update(groupMember);
            // update Group Member Position
            _unitOfWork.Repository<GroupMemberPosition>().AddRange(positionsToAdd);
            _unitOfWork.Repository<GroupMemberPosition>().DeleteRange(positionsToRemove);
            await _unitOfWork.Repository<GroupMemberPosition>().SaveAllAsync();
        }

        public async Task UpdateGroupPositionAsync(GroupPosition groupPosition, IEnumerable<int> skillIds)
        {
            var existingSkills = await _unitOfWork.Repository<GroupPositionSkill>()
                                                  .ListAsync(new GroupPositionSkillSpecification(groupPosition.Id));
            var existingSkillIds = existingSkills.Select(s => s.SkillId).ToList();

            var skillsToAdd = skillIds.Except(existingSkillIds)
                              .Select(skillId => new GroupPositionSkill
                              {
                                  GroupPositionId = groupPosition.Id,
                                  SkillId = skillId
                              });
            var skillsToRemove = existingSkills.Where(s => !skillIds.Contains(s.SkillId));


            // update GroupPosition
            _unitOfWork.Repository<GroupPosition>().Update(groupPosition);
            await _unitOfWork.Repository<GroupPosition>().SaveAllAsync();

            // add GroupPositionSkill
            if (skillsToAdd.Count() > 0) 
                _unitOfWork.Repository<GroupPositionSkill>().AddRange(skillsToAdd);
            if(skillsToRemove.Count() > 0) 
                _unitOfWork.Repository<GroupPositionSkill>().DeleteRange(skillsToRemove);

            await _unitOfWork.Repository<GroupPositionSkill>().SaveAllAsync();

        }
    }
}
