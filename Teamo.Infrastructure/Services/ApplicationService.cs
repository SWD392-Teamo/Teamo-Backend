using Microsoft.VisualBasic;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Applications;
using Teamo.Core.Specifications.Groups;

namespace Teamo.Infrastructure.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }   

        public async Task<Application> GetApplicationByIdAsync(int id)
        {
            var appSpec = new ApplicationSpecification(id);
            return await _unitOfWork.Repository<Application>().GetEntityWithSpec(appSpec);
        }

        public async Task<IReadOnlyList<Application>> GetSentApplicationsAsync(ApplicationSpecification appSpec)
        {
            return await _unitOfWork.Repository<Application>().ListAsync(appSpec);
        }

        public async Task<IReadOnlyList<Application>> GetGroupApplicationsAsync(ApplicationGroupSpecification appSpec)
        {
            return await _unitOfWork.Repository<Application>().ListAsync(appSpec);
        }

        public async Task<bool> ReviewApplicationAsync(Application app, string newStatus)
        {   
            if(Enum.TryParse(newStatus, out ApplicationStatus appStatus))
            {
                app.Status = appStatus;
                _unitOfWork.Repository<Application>().Update(app);
                return await _unitOfWork.Complete();
            }
            else return false;
        }

        public async Task<bool> CreateNewApplicationAsync(Application newAapp)
        {
            _unitOfWork.Repository<Application>().Add(newAapp);
            return await _unitOfWork.Complete();            
        }

        public async Task<string> GetGroupLeaderEmailAsync(int groupId)
        {
            var groupSpec = new GroupSpecification(groupId);
            var group = await _unitOfWork.Repository<Group>().GetEntityWithSpec(groupSpec);

            var memberSpec = new GroupMemberSpecification(groupId, GroupMemberRole.Leader);
            var groupLeader = await _unitOfWork.Repository<GroupMember>().GetEntityWithSpec(memberSpec);

            return groupLeader.Student.Email;
        }

        public async Task<bool> CheckValidToApply(int groupId, int studentId, int groupPositionId)
        {
            var isValid = true;

            //Check for group status
            var groupSpec = new GroupSpecification(groupId);
            var group = await _unitOfWork.Repository<Group>().GetEntityWithSpec(groupSpec);
            if(
                group == null 
                || group.Status == GroupStatus.Archived
                || group.Status == GroupStatus.Full
            ) isValid = false;

            //Check if student is already a member of group
            var memberSpec = new GroupMemberSpecification(groupId, studentId);
            var existMember = await _unitOfWork.Repository<GroupMember>().GetEntityWithSpec(memberSpec);
            if(existMember != null) isValid = false;

            //Check for position status
            var posSpec = new GroupPositionSpecification(groupPositionId);
            var position = await _unitOfWork.Repository<GroupPosition>().GetEntityWithSpec(posSpec);
            if(
                position == null
                || position.Status == GroupPositionStatus.Closed
            ) isValid = false;

            return isValid;
        }

        public async Task<int> CountAsync(ApplicationSpecification appSpec)
        {
            return await _unitOfWork.Repository<Application>().CountAsync(appSpec);
        }

        public async Task<int> CountAsync(ApplicationGroupSpecification appSpec)
        {
            return await _unitOfWork.Repository<Application>().CountAsync(appSpec);
        }
    }
}