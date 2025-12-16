using AuthSystem.Application.DTOs;
using FluentAssertions;

namespace AuthSystem.Application.UnitTests.DTOs;

public class AuthDTOsTests
{
    [Fact]
    public void LoginDto_Constructor_SetsProperties()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";

        // Act
        var loginDto = new LoginDto(email, password);

        // Assert
        loginDto.Email.Should().Be(email);
        loginDto.Password.Should().Be(password);
    }

    [Fact]
    public void RegisterDto_Constructor_SetsProperties()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";

        // Act
        var registerDto = new RegisterDto(email, password);

        // Assert
        registerDto.Email.Should().Be(email);
        registerDto.Password.Should().Be(password);
    }

    [Fact]
    public void AuthResponseDto_Constructor_SetsProperties()
    {
        // Arrange
        var token = "jwt-token";
        var refreshToken = "refresh-token";
        var expiration = DateTime.UtcNow.AddMinutes(30);

        // Act
        var authResponseDto = new AuthResponseDto(token, refreshToken, expiration);

        // Assert
        authResponseDto.Token.Should().Be(token);
        authResponseDto.RefreshToken.Should().Be(refreshToken);
        authResponseDto.Expiration.Should().Be(expiration);
    }

    [Fact]
    public void RefreshTokenDto_Constructor_SetsProperties()
    {
        // Arrange
        var token = "jwt-token";
        var refreshToken = "refresh-token";

        // Act
        var refreshTokenDto = new RefreshTokenDto(token, refreshToken);

        // Assert
        refreshTokenDto.Token.Should().Be(token);
        refreshTokenDto.RefreshToken.Should().Be(refreshToken);
    }

    [Fact]
    public void LoginDto_Equality_WorksCorrectly()
    {
        // Arrange
        var dto1 = new LoginDto("test@example.com", "password");
        var dto2 = new LoginDto("test@example.com", "password");
        var dto3 = new LoginDto("different@example.com", "password");

        // Act & Assert
        dto1.Should().Be(dto2);
        dto1.Should().NotBe(dto3);
    }

    [Fact]
    public void RegisterDto_Equality_WorksCorrectly()
    {
        // Arrange
        var dto1 = new RegisterDto("test@example.com", "password");
        var dto2 = new RegisterDto("test@example.com", "password");
        var dto3 = new RegisterDto("different@example.com", "password");

        // Act & Assert
        dto1.Should().Be(dto2);
        dto1.Should().NotBe(dto3);
    }
}