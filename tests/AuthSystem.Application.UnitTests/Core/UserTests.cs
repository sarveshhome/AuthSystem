using AuthSystem.Core.Entities;
using FluentAssertions;

namespace AuthSystem.Application.UnitTests.Core;

public class UserTests
{
    [Fact]
    public void User_DefaultRole_ShouldBeUser()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Role.Should().Be("User");
    }

    [Fact]
    public void User_SetProperties_ShouldRetainValues()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var passwordHash = "hashedpassword";
        var refreshToken = "refreshtoken";
        var expiryTime = DateTime.UtcNow.AddDays(7);

        // Act
        var user = new User
        {
            Id = userId,
            Email = email,
            PasswordHash = passwordHash,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = expiryTime,
            Role = "Admin"
        };

        // Assert
        user.Id.Should().Be(userId);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
        user.RefreshToken.Should().Be(refreshToken);
        user.RefreshTokenExpiryTime.Should().Be(expiryTime);
        user.Role.Should().Be("Admin");
    }

    [Fact]
    public void User_RefreshToken_CanBeNull()
    {
        // Arrange & Act
        var user = new User
        {
            RefreshToken = null
        };

        // Assert
        user.RefreshToken.Should().BeNull();
    }

    [Fact]
    public void User_RefreshTokenExpiryTime_CanBeNull()
    {
        // Arrange & Act
        var user = new User
        {
            RefreshTokenExpiryTime = null
        };

        // Assert
        user.RefreshTokenExpiryTime.Should().BeNull();
    }
}