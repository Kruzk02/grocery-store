using Domain.Entity;

namespace API.Services;

public interface IVerificationTokenService
{
    Task<VerificationToken> GenerateVerificationToken(ApplicationUser user);
    Task<bool> VerifyToken(string token);
}