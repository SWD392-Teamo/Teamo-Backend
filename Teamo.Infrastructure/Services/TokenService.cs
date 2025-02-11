using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Interfaces.Services;

namespace Teamo.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]));
        }

        public (string token, DateTime expires) GenerateToken(User user, string userRole)
        {
            // Added claims to the token
            var claims = new List<Claim>
            {
                // Add identifier in Claim in order to retrieve user
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, userRole)
            };

            // Sign the token
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Set expiration time
            var expiration = DateTime.Now.AddDays(1);

            // Create token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = creds,
                Issuer = _config["Token:Issuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            // Generate jwt access token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return (tokenHandler.WriteToken(token), expiration);
        }
    }
}
