using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Security
{
    public class JwtTokenProvider
    {
        public static string CreateToken(string secureKey, int expirationMinutes, string username, string uloga)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secureKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, uloga)
            };

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}