
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications;

namespace Teamo.Infrastructure.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        public GroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateGroupAsync(Group group)
        {
            _unitOfWork.Repository<Group>().Add(group);
            return await _unitOfWork.Repository<Group>().SaveAllAsync();
        }

        public async Task<Group> GetGroupByIdAsync(ISpecification<Group> spec)
        {
            return await _unitOfWork.Repository<Group>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Group>> GetGroupsAsync(ISpecification<Group> spec)
        {
            return await _unitOfWork.Repository<Group>().ListAsync(spec);
        }

        public async Task<bool> UpdateGroupAsync(Group group)
        {
            _unitOfWork.Repository<Group>().Update(group);
            return await _unitOfWork.Repository<Group>().SaveAllAsync();
        }
    }
}
