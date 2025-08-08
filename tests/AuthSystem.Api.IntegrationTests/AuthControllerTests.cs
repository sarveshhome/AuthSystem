using AuthSystem.Api.Controllers;
using AuthSystem.Application.DTOs;
using AuthSystem.Application.Interfaces;
using AuthSystem.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AuthSystem.Api.IntegrationTests;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var loginDto = new LoginDto("test@example.com", "password");
        var expectedResponse = new AuthResponseDto("token", "refreshToken", DateTime.UtcNow.AddMinutes(30));
        
        _mockAuthService.Setup(x => x.LoginAsync(loginDto))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal(expectedResponse.Token, response.Token);
    }

    [Fact]
    public async Task Register_NewUser_ReturnsOkWithToken()
    {
        // Arrange
        var registerDto = new RegisterDto("new@example.com", "password");
        var expectedResponse = new AuthResponseDto("token", "refreshToken", DateTime.UtcNow.AddMinutes(30));
        
        _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal(expectedResponse.Token, response.Token);
    }

    [Fact]
    public async Task RefreshToken_ValidTokens_ReturnsNewTokens()
    {
        // Arrange
        var refreshTokenDto = new RefreshTokenDto("expiredToken", "refreshToken");
        var expectedResponse = new AuthResponseDto("newToken", "newRefreshToken", DateTime.UtcNow.AddMinutes(30));
        
        _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenDto))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.RefreshToken(refreshTokenDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal(expectedResponse.Token, response.Token);
    }
}