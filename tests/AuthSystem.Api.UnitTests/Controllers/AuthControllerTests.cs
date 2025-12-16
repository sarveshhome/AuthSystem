using AuthSystem.Api.Controllers;
using AuthSystem.Application.DTOs;
using AuthSystem.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuthSystem.Api.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var loginDto = new LoginDto("test@example.com", "password123");
        var authResponse = new AuthResponseDto("token", "refresh-token", DateTime.UtcNow.AddMinutes(30));
        
        _authServiceMock.Setup(x => x.LoginAsync(loginDto))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(authResponse);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto("test@example.com", "wrongpassword");
        
        _authServiceMock.Setup(x => x.LoginAsync(loginDto))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.Login(loginDto));
    }

    [Fact]
    public async Task Register_ValidData_ReturnsOkResult()
    {
        // Arrange
        var registerDto = new RegisterDto("newuser@example.com", "password123");
        var authResponse = new AuthResponseDto("token", "refresh-token", DateTime.UtcNow.AddMinutes(30));
        
        _authServiceMock.Setup(x => x.RegisterAsync(registerDto))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(authResponse);
    }

    [Fact]
    public async Task Register_ExistingEmail_ThrowsArgumentException()
    {
        // Arrange
        var registerDto = new RegisterDto("existing@example.com", "password123");
        
        _authServiceMock.Setup(x => x.RegisterAsync(registerDto))
            .ThrowsAsync(new ArgumentException("Email already exists"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _controller.Register(registerDto));
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsOkResult()
    {
        // Arrange
        var refreshTokenDto = new RefreshTokenDto("expired-token", "valid-refresh-token");
        var authResponse = new AuthResponseDto("new-token", "new-refresh-token", DateTime.UtcNow.AddMinutes(30));
        
        _authServiceMock.Setup(x => x.RefreshTokenAsync(refreshTokenDto))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.RefreshToken(refreshTokenDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(authResponse);
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_ThrowsSecurityTokenException()
    {
        // Arrange
        var refreshTokenDto = new RefreshTokenDto("invalid-token", "invalid-refresh-token");
        
        _authServiceMock.Setup(x => x.RefreshTokenAsync(refreshTokenDto))
            .ThrowsAsync(new Microsoft.IdentityModel.Tokens.SecurityTokenException("Invalid token"));

        // Act & Assert
        await Assert.ThrowsAsync<Microsoft.IdentityModel.Tokens.SecurityTokenException>(() => _controller.RefreshToken(refreshTokenDto));
    }

    [Fact]
    public void AdminOnly_ReturnsOkResult()
    {
        // Act
        var result = _controller.AdminOnly();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be("This is accessible only by Admin");
    }

    [Fact]
    public void AuthenticatedOnly_ReturnsOkResult()
    {
        // Act
        var result = _controller.AuthenticatedOnly();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be("This is accessible by any authenticated user");
    }
}