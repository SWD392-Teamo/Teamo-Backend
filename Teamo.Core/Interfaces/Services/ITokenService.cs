using Teamo.Core.Entities.Identity;

namespace Teamo.Core.Interfaces.Services
{
    public interface ITokenService
    {
        (string token, DateTime expires) GenerateToken(User user, string userRole);
    }
}
