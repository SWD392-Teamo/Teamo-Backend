using Teamo.Core.Entities;

namespace Teamo.Core.Interfaces.Services
{
    public interface INotificationService
    {
        Task<bool> SendNotificationAsync(FCMessage message);
    }
}
