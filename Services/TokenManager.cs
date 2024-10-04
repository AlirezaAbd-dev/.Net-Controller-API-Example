using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cityinfo.API.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Cityinfo.API.Services;

public class TokenManager(IConfiguration config) : ITokenManager
{
    public string AssignToken(int id, string name, Role role) {
        var securityKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(config["Authentication:SecretForKey"]!)
        );
        var signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );
        var claimsForToken = new List<Claim> {
            new Claim("id", id.ToString()),
            new Claim("name", name),
            new Claim("role", role.ToString())
        };

        var jwtSecurityToken = new JwtSecurityToken(
            config["Authentication:Issuer"],
            config["Authentication:Audience"],
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials
        );

        var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return tokenToReturn;
    }
}
