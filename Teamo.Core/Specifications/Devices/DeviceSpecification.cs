using Teamo.Core.Entities;

namespace Teamo.Core.Specifications.Devices
{
    public class DeviceSpecification : BaseSpecification<UserDevice>
    {
        public DeviceSpecification(string token)
            : base(x => x.FCMToken.Equals(token))
        {
        }

        public DeviceSpecification(int userId)
            : base(x => x.UserId == userId)
        {
        }
    }
}
