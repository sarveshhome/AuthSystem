using AuthSystem.Application.DTOs;
using AuthSystem.Application.Services;
using AuthSystem.Core.Entities;
using AuthSystem.Core.Interfaces;
using AuthSystem.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AuthSystem.Application.UnitTests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly AuthDbContext _context;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AuthDbContext(options);
        _jwtServiceMock = new Mock<IJwtService>();
        _authService = new AuthService(_context, _jwtServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
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
        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("test-token");
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("test-token");
        result.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task LoginAsync_InvalidEmail_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new LoginDto("nonexistent@example.com", "password123");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            Role = "User"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var loginDto = new LoginDto("test@example.com", "wrongpassword");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task RegisterAsync_ValidData_ReturnsAuthResponse()
    {
        // Arrange
        var registerDto = new RegisterDto("newuser@example.com", "password123");
        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("test-token");
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("test-token");
        result.RefreshToken.Should().Be("refresh-token");
        
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
        user.Should().NotBeNull();
        user!.Role.Should().Be("User");
    }

    [Fact]
    public async Task RegisterAsync_ExistingEmail_ThrowsArgumentException()
    {
        // Arrange
        var existingUser = new User
        {
            Email = "existing@example.com",
            PasswordHash = "hash",
            Role = "User"
        };
        await _context.Users.AddAsync(existingUser);
        await _context.SaveChangesAsync();

        var registerDto = new RegisterDto("existing@example.com", "password123");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterAsync(registerDto));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}