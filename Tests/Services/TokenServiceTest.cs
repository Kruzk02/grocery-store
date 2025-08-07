using System.IdentityModel.Tokens.Jwt;
using API.Entity;
using API.Services;
using API.Settings;
using Microsoft.Extensions.Options;

namespace Tests.Services;

[TestFixture]
public class TokenServiceTest
{
    [Test]
    public void CreateToken_ShouldReturn_ValidJwtToken()
    {
        var jwtSettings = new JwtSettings
        {
            Key = "supersecretkey12345678901234567890",
            Issuer = "test-issuer",
            Audience = "test-audience"
        };

        var options = Options.Create(jwtSettings);

        var tokenService = new TokenService(options);

        var user = new ApplicationUser
        {
            Id = "123",
            UserName = "testuser",
            Email = "test@example.com"
        };
        
        var token = tokenService.CreateToken(user);

        Assert.That(string.IsNullOrWhiteSpace(token), Is.False);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        
        Assert.That(user.Id, Is.EqualTo(jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value));
        Assert.That(user.UserName, Is.EqualTo(jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value));
        Assert.That(user.Email, Is.EqualTo(jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value));
    }
}