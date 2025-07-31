namespace API.Dtos;

public record LoginResponse( bool Success, string Token, string ErrorMessage);