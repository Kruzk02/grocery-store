using API.Data;
using API.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class VerificationTokenService(ApplicationDbContext ctx) : IVerificationTokenService
{
    public async Task<VerificationToken> GenerateVerificationToken(ApplicationUser user)
    {
        var token = new VerificationToken
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };

        ctx.VerificationTokens.Add(token);
        await ctx.SaveChangesAsync();

        return token;
    }

    public async Task<bool> VerifyToken(string token)
    {
        var verificationToken = await ctx.VerificationTokens
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.Token == token);

        if (verificationToken == null || verificationToken.ExpiresAt < DateTime.UtcNow) return false;
        
        verificationToken.User.EmailConfirmed = true;
        ctx.Users.Update(verificationToken.User);
        ctx.VerificationTokens.Remove(verificationToken);
        await ctx.SaveChangesAsync();
        
        Console.WriteLine($"EmailConfirmed after SaveChanges: {verificationToken.User.EmailConfirmed}");

        return true;
    }
}