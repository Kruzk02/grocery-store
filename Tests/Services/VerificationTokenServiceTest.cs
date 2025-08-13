using API.Data;
using API.Entity;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace Tests.Services;

[TestFixture]
public class VerificationTokenServiceTest
{

    private static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
    
    [Test]
    public async Task GenerateVerificationToken_ShouldCreateToken()
    {
        var ctx = GetInMemoryDbContext();
        var service = new VerificationTokenService(ctx);
        var user = new ApplicationUser{Id = "123"};
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var token = await service.GenerateVerificationToken(user);
        
        Assert.Multiple(() =>
        {
            Assert.That(token, !Is.Null);
            Assert.That(token.UserId, Is.EqualTo("123"));
            Assert.That((token.ExpiresAt - DateTime.UtcNow).TotalMinutes, Is.LessThan(10.1));
            Assert.That(ctx.VerificationTokens, !Is.Empty);
        });
    }

    [Test]
    public async Task VerifyToken_ShouldConfirmEmailAndRemoveToken_WhenValid()
    {
        var ctx = GetInMemoryDbContext();
        var user = new ApplicationUser{ Id = "123", EmailConfirmed = false};
        var token = new VerificationToken
        {
            Token = "token",
            UserId = "123",
            User = user,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };
        ctx.Users.Add(user);
        ctx.VerificationTokens.Add(token);
        await ctx.SaveChangesAsync();

        var service = new VerificationTokenService(ctx);

        var result = await service.VerifyToken("token");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(user.EmailConfirmed, Is.True);
            Assert.That(ctx.VerificationTokens, Is.Empty);
        });
    }

    [Test]
    public async Task VerifyToken_ShouldNotConfirmEmail_WhenTokenExpired()
    {
        var ctx = GetInMemoryDbContext();
        var user = new ApplicationUser{ Id = "123", EmailConfirmed = false};
        var token = new VerificationToken
        {
            Token = "token",
            UserId = "123",
            User = user,
            ExpiresAt = DateTime.UtcNow.AddMinutes(-5)
        };
        ctx.Users.Add(user);
        ctx.VerificationTokens.Add(token);
        await ctx.SaveChangesAsync();

        var service = new VerificationTokenService(ctx);

        var result = await service.VerifyToken("token");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(user.EmailConfirmed, Is.False);
            Assert.That(ctx.VerificationTokens, !Is.Empty);
        });
    }
    
    [Test]
    public async Task VerifyToken_ShouldNotConfirmEmail_WhenTokenNotFound()
    {
        var ctx = GetInMemoryDbContext();
        var service = new VerificationTokenService(ctx);
        
        var result = await service.VerifyToken("token");
        
        Assert.That(result, Is.False);
    }
}