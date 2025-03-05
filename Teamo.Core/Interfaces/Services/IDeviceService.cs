using Teamo.Core.Entities;

namespace Teamo.Core.Interfaces.Services
{
    public interface IDeviceService
    {
        Task<bool> AddDeviceAsync(UserDevice device);
        Task<List<string>> GetDeviceTokensForUser(int userId);
        Task<List<string>> GetDeviceTokensForSelectedUsers(List<int> userIds);
        Task<List<string>> GetAllDeviceTokens();
    }
}
