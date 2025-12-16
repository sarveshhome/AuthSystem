using System.Security.Claims;
using AuthSystem.Application.DTOs;
using AuthSystem.Application.Services;
using AuthSystem.Core.Entities;
using AuthSystem.Infrastructure.Data;
using AuthSystem.Infrastructure.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthSystem.Application.UnitTests.Services;

public class AuthServiceIntegrationTests : IDisposable
{
    private readonly AuthDbContext _context;
    private readonly JwtService _jwtService;
    private readonly AuthService _authService;

    public AuthServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AuthDbContext(options);
        _jwtService = new JwtService("this-is-a-very-long-secret-key-for-testing-purposes-123456789", "TestIssuer", 30, 7);
        _authService = new AuthService(_context, _jwtService);
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidRefreshToken_ReturnsNewTokens()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = "User",
            RefreshToken = "valid-refresh-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        
        // Verify token can be validated
        var principal = _jwtService.GetPrincipalFromExpiredToken(token);
        principal.Should().NotBeNull();
        
        var userIdClaim = principal!.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        userIdClaim.Should().NotBeNull();
        userIdClaim.Should().Be(user.Id.ToString());
        
        var refreshTokenDto = new RefreshTokenDto(token, "valid-refresh-token");

        // Act
        var result = await _authService.RefreshTokenAsync(refreshTokenDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBe("valid-refresh-token"); // Should be a new refresh token
    }

    [Fact]
    public async Task RefreshTokenAsync_ExpiredRefreshToken_ThrowsSecurityTokenException()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = "User",
            RefreshToken = "expired-refresh-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1) // Expired
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        var refreshTokenDto = new RefreshTokenDto(token, "expired-refresh-token");

        // Act & Assert
        await Assert.ThrowsAsync<SecurityTokenException>(() => _authService.RefreshTokenAsync(refreshTokenDto));
    }

    [Fact]
    public async Task RefreshTokenAsync_MismatchedRefreshToken_ThrowsSecurityTokenException()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = "User",
            RefreshToken = "correct-refresh-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        var refreshTokenDto = new RefreshTokenDto(token, "wrong-refresh-token");

        // Act & Assert
        await Assert.ThrowsAsync<SecurityTokenException>(() => _authService.RefreshTokenAsync(refreshTokenDto));
    }

    [Fact]
    public async Task LoginAsync_UpdatesRefreshTokenInDatabase()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = "User"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var loginDto = new LoginDto("test@example.com", "password123");

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        var updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        updatedUser!.RefreshToken.Should().NotBeNullOrEmpty();
        updatedUser.RefreshTokenExpiryTime.Should().BeAfter(DateTime.UtcNow);
        updatedUser.RefreshToken.Should().Be(result.RefreshToken);
    }

    [Fact]
    public async Task RegisterAsync_CreatesUserWithHashedPassword()
    {
        // Arrange
        var registerDto = new RegisterDto("newuser@example.com", "password123");

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
        user.Should().NotBeNull();
        user!.PasswordHash.Should().NotBe("password123"); // Should be hashed
        BCrypt.Net.BCrypt.Verify("password123", user.PasswordHash).Should().BeTrue();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}