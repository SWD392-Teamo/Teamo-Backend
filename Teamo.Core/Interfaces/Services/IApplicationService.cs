using Teamo.Core.Entities;
using Teamo.Core.Specifications.Applications;

namespace Teamo.Core.Interfaces.Services
{
    public interface IApplicationService
    {
        Task<Application> GetApplicationByIdAsync(int id);
        Task<IReadOnlyList<Application>> GetSentApplicationsAsync(ApplicationSpecification appSpec);
        Task<IReadOnlyList<Application>> GetGroupApplicationsAsync(ApplicationGroupSpecification appSpec);
        Task<bool> ReviewApplicationAsync(Application app);
        Task<bool> CreateNewApplicationAsync(Application newAapp);
        Task<bool> DeleteApplicationAsync(Application app);
        Task<int> GetGroupLeaderIdAsync(int groupId);
        Task<bool> CheckValidToApply(int groupId, int studentId, int groupPositionId);
        Task<int> CountAsync(ApplicationSpecification appSpec);
        Task<int> CountAsync(ApplicationGroupSpecification appSpec);
    }
}