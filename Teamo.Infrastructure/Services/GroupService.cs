
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

            var groupPositionSpec = new GroupPositionSpecification(new GroupPositionParams
            {
                GroupId = groupMember.GroupId,
                PositionId = groupMember.GroupPositionId
            });
            var groupPositon = await _unitOfWork.Repository<GroupPosition>().GetEntityWithSpec(groupPositionSpec);
            if (groupPositon == null)
            {
                throw new InvalidOperationException("This group does not have this position");
            }

            groupMember.Role = GroupMemberRole.Member;
            _unitOfWork.Repository<GroupMember>().Add(groupMember);
            await _unitOfWork.Repository<GroupMember>().SaveAllAsync();
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

            var groupPositionSpec = new GroupPositionSpecification(new GroupPositionParams
            {
                GroupId = groupMember.GroupId,
                PositionId = groupMember.GroupPositionId
            });
            var groupPositon = await _unitOfWork.Repository<GroupPosition>().GetEntityWithSpec(groupPositionSpec);
            if (groupPositon == null)
            {
                throw new InvalidOperationException("This group does not have this position");
            }

            groupMember.Role = GroupMemberRole.Member;
            _unitOfWork.Repository<GroupMember>().Add(groupMember);
            await _unitOfWork.Repository<GroupMember>().SaveAllAsync();
        }

        public async Task CreateGroupAsync(Group group, int createdUserId, IEnumerable<GroupPosition> groupPositions)
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
            group.Status = GroupStatus.Archived;
            _unitOfWork.Repository<Group>().Update(group);
            await _unitOfWork.Repository<Group>().SaveAllAsync();
        }

        public async Task<Group> GetGroupByIdAsync(int id)
        {
            var spec = new GroupSpecification(id);
            return await _unitOfWork.Repository<Group>().GetEntityWithSpec(spec);
        }

        public async Task<GroupMember> GetGroupMemberByIdAsync(int groupMemberId)
        {
            return await _unitOfWork.Repository<GroupMember>().GetByIdAsync(groupMemberId);
        }

        public async Task<GroupPosition> GetGroupPositionByIdAsync(int id)
        {
            var spec = new GroupPositionSpecification(id);
            return await _unitOfWork.Repository<GroupPosition>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Group>> GetGroupsAsync(ISpecification<Group> spec)
        {
            return await _unitOfWork.Repository<Group>().ListAsync(spec);
        }

        public async Task<IReadOnlyList<Group>> GetGroupsByMemberIdAsync(ISpecification<GroupMember> spec)
        {
            var studentGroups = await _unitOfWork.Repository<GroupMember>().ListAsync(spec);
            var groups = new List<Group>(); 
            foreach (var sg in studentGroups)
            {
                var group = await GetGroupByIdAsync(sg.Id);
                groups.Add(group);
            }
            return groups;
        }

        public async Task RemoveMemberFromGroup(GroupMember groupMember)
        {
            _unitOfWork.Repository<GroupMember>().Delete(groupMember);
            await _unitOfWork.Repository<GroupMember>().SaveAllAsync();
        }

        public async Task UpdateGroupAsync(Group group)
        {
            _unitOfWork.Repository<Group>().Update(group);
            await _unitOfWork.Repository<Group>().SaveAllAsync();
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
