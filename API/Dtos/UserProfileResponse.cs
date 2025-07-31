using API.Entity;

namespace API.Dtos;

public record UserProfileResponse(bool Success, ApplicationUser User, string ErrorMessage);