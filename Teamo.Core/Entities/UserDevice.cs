using Teamo.Core.Entities.Identity;

namespace Teamo.Core.Entities
{
    public class UserDevice : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string FCMToken { get; set; }
    }
}
