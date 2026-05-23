namespace Application.DTOs.Response;

public record AuthResponse(string AccessToken, string RefreshToken, DateTime RefreshTokenExpiry);
