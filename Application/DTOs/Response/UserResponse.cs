using Domain.Entity;

namespace Application.DTOs.Response;

public record UserResponse(string Id, string Username, string Email, IList<string> Roles)
{
    public static UserResponse FromEntity(User user)
    {
        return new UserResponse(user.Id!, user.Username!, user.Email!, user.Roles!);
    }
}
