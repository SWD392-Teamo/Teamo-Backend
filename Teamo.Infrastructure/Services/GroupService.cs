
using Teamo.Core.Entities;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications;
using Teamo.Core.Specifications.Groups;

namespace Teamo.Infrastructure.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        public GroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateGroupAsync(Group group, int createdUserId)
        {
            group.CreatedById = createdUserId;
            _unitOfWork.Repository<Group>().Add(group);
            await _unitOfWork.Repository<Group>().SaveAllAsync();
        }

        public async Task<Group> GetGroupByIdAsync(int id)
        {
            var spec = new GroupSpecification(id);
            return await _unitOfWork.Repository<Group>().GetEntityWithSpec(spec);
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
    }
}
