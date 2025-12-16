using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthSystem.Application.DTOs;
using AuthSystem.Application.Interfaces;
using AuthSystem.Core.Entities;
using AuthSystem.Core.Interfaces;
using AuthSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthSystem.Application.Services;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(AuthDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync();

        return new AuthResponseDto(token, refreshToken, DateTime.UtcNow.AddMinutes(30));
    }

    // AuthSystem.Application/Services/AuthService.cs
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            throw new ArgumentException("Email already exists");

        var user = new User
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Role = "User",
            RefreshToken = "", // Initialize with empty string if you don't want null
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync();

        return new AuthResponseDto(token, refreshToken, DateTime.UtcNow.AddMinutes(30));
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(refreshTokenDto.Token);

        if (principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value is not string userId)
            throw new SecurityTokenException("Invalid token");

        if (!Guid.TryParse(userId, out var userGuid))
            throw new SecurityTokenException("Invalid token");

        var users = await _context.Users.ToListAsync();
        var user = users.FirstOrDefault(u => u.Id.ToString() == userId);

        if (
            user == null
            || user.RefreshToken != refreshTokenDto.RefreshToken
            || user.RefreshTokenExpiryTime <= DateTime.UtcNow
        )
            throw new SecurityTokenException("Invalid refresh token");

        var newToken = _jwtService.GenerateToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync();

        return new AuthResponseDto(newToken, newRefreshToken, DateTime.UtcNow.AddMinutes(30));
    }
}
