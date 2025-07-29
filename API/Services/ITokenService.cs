using API.Entity;

namespace API.Services;

public interface ITokenService
{
    string CreateToken(ApplicationUser user);
}