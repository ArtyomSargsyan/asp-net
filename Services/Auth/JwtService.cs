using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ToDoApi.Models;

namespace ToDoApi.Services.Auth
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly int _expiryHours;

        public JwtService(IConfiguration config)
        {
            _config = config;
            _expiryHours = int.TryParse(_config["Jwt:ExpiryHours"], out var h) ? h : 2;
        }

        public string GenerateToken(User user)
        {
            // 1. Create claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            // 2. Signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Create token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_expiryHours),
                signingCredentials: creds
            );

            // 4. Write token as string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
