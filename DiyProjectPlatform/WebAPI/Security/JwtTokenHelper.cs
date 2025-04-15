using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Security;

public class JwtTokenHelper
{
    // TODO: move params to JwtTokenOptions class
    public static string CreateToken(string secureKey, int expiration, string name = null, string subject = null, string role = null)
    {
        // Get secret key bytes
        var tokenKey = Encoding.UTF8.GetBytes(secureKey);

        // Create a token descriptor (represents a token, kind of a "template" for token)
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMinutes(expiration),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        if (!string.IsNullOrEmpty(subject))
        {
            tokenDescriptor.Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, name ?? subject),
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim(ClaimTypes.Role, role)
            });
        }

        // Create token using that descriptor, serialize it and return it
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var serializedToken = tokenHandler.WriteToken(token);

        return serializedToken;
    }
}