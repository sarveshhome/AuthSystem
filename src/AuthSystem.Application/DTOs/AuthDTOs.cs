namespace AuthSystem.Application.DTOs;

public record LoginDto(string Email, string Password);
public record RegisterDto(string Email, string Password);
public record AuthResponseDto(string Token, string RefreshToken, DateTime Expiration);
public record RefreshTokenDto(string Token, string RefreshToken);