using AuthSystem.Core.Entities;
using AuthSystem.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Application.UnitTests.Infrastructure;

public class AuthDbContextTests : IDisposable
{
    private readonly AuthDbContext _context;

    public AuthDbContextTests()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AuthDbContext(options);
    }

    [Fact]
    public async Task AddUser_ShouldSaveToDatabase()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            Role = "User"
        };

        // Act
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be(user.Email);
        savedUser.Role.Should().Be("User");
    }

    [Fact]
    public async Task User_DefaultRole_ShouldBeUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };

        // Act
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        savedUser!.Role.Should().Be("User");
    }

    [Fact]
    public async Task UpdateUser_ShouldPersistChanges()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            Role = "User"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        user.RefreshToken = "new-refresh-token";
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        // Assert
        var updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        updatedUser!.RefreshToken.Should().Be("new-refresh-token");
        updatedUser.RefreshTokenExpiryTime.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task FindUserByEmail_ShouldReturnCorrectUser()
    {
        // Arrange
        var user1 = new User { Id = Guid.NewGuid(), Email = "user1@example.com", PasswordHash = "hash1", Role = "User" };
        var user2 = new User { Id = Guid.NewGuid(), Email = "user2@example.com", PasswordHash = "hash2", Role = "Admin" };
        
        await _context.Users.AddRangeAsync(user1, user2);
        await _context.SaveChangesAsync();

        // Act
        var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "user2@example.com");

        // Assert
        foundUser.Should().NotBeNull();
        foundUser!.Email.Should().Be("user2@example.com");
        foundUser.Role.Should().Be("Admin");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}