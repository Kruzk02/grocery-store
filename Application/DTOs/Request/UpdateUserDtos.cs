namespace Application.DTOs.Request;

public record UpdateUserDto(string? Username, string? Email, string? Password, string? Role);
