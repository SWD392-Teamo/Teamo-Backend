using FirebaseAdmin.Messaging;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces.Services;

namespace Teamo.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly FirebaseMessaging _messaging;

        public NotificationService()
        {
            _messaging = FirebaseMessaging.DefaultInstance;
        }

        public async Task<bool> SendNotificationAsync(FCMessage fcMessage)
        {
            // Multicast message to multiple device incase one user have many devices
            var message = new MulticastMessage()
            {
                Tokens = fcMessage.tokens,
                Notification = new Notification()
                {
                    Title = fcMessage.title,
                    Body = fcMessage.body,
                },
                Data = fcMessage.data
            };

            // Multicast message to specified device
            var response = await _messaging.SendEachForMulticastAsync(message);

            if (response.FailureCount > 0)
            {
                return false;
            }

            return true;
        }
    }
}
