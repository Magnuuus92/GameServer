using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GameServer.Services
{
    public class JwtService
    {
        private readonly string _secret;
        private readonly int _expiryDays;

        public JwtService(Iconfiguration config)
        {
            _secret  = config["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT secret not configured.");
            _expiryDays = int.Parse(config["Jwt:ExpiryDays"] ?? "30");
        }

        //Generate token with user id and username
        public string GenerateToken(string userId, string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
            };
        }
    }
}