using API.Entity;
using Microsoft.AspNetCore.Identity;

namespace API.Services;

public interface ITokenService
{
    Task<String> CreateToken(ApplicationUser user, UserManager<ApplicationUser> userManager);
}