using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Config;

namespace WebAPI.Helpers;

public class JwtTokenHelper
{
    public static string CreateToken(string name, string subject, string role)
    {
        // Get secret key bytes
        var tokenKey = Encoding.UTF8.GetBytes(JwtTokenConfig.TokenSecret);

        // Create a token descriptor (represents a token, kind of a "template" for token)
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMinutes(JwtTokenConfig.TokenExpiration),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        if (!string.IsNullOrEmpty(subject))
        {
            tokenDescriptor.Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, name),
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