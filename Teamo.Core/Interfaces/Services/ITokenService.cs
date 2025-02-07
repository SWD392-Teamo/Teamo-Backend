using Teamo.Core.Entities.Identity;

namespace Teamo.Core.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user, string userRole);
    }
}
