using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Entity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        };

        var token = new JwtSecurityToken(
            issuer: config["JwtSettings:Issuer"],
            audience: config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: cred);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}