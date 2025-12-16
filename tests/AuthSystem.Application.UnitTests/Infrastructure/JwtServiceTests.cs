using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthSystem.Core.Entities;
using AuthSystem.Infrastructure.Services;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;

namespace AuthSystem.Application.UnitTests.Infrastructure;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;
    private readonly string _secretKey = "this-is-a-very-long-secret-key-for-testing-purposes-123456789";
    private readonly string _issuer = "TestIssuer";

    public JwtServiceTests()
    {
        _jwtService = new JwtService(_secretKey, _issuer, 30, 7);
    }

    [Fact]
    public void GenerateToken_ValidUser_ReturnsValidJwtToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Role = "User"
        };

        // Act
        var token = _jwtService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var claims = jwtToken.Claims.ToList();
        claims.Should().Contain(c => c.Type == "nameid" && c.Value == user.Id.ToString());
        claims.Should().Contain(c => c.Type == "email" && c.Value == user.Email);
        claims.Should().Contain(c => c.Type == "role" && c.Value == user.Role);
        jwtToken.Issuer.Should().Be(_issuer);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsBase64String()
    {
        // Act
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        refreshToken.Should().MatchRegex(@"^[A-Za-z0-9+/]*={0,2}$"); // Base64 pattern
    }

    [Fact]
    public void GenerateRefreshToken_GeneratesUniqueTokens()
    {
        // Act
        var token1 = _jwtService.GenerateRefreshToken();
        var token2 = _jwtService.GenerateRefreshToken();

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ValidToken_ReturnsPrincipal()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Role = "Admin"
        };
        var token = _jwtService.GenerateToken(user);

        // Act
        var principal = _jwtService.GetPrincipalFromExpiredToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.FindFirst(ClaimTypes.NameIdentifier)?.Value.Should().Be(user.Id.ToString());
        principal.FindFirst(ClaimTypes.Email)?.Value.Should().Be(user.Email);
        principal.FindFirst(ClaimTypes.Role)?.Value.Should().Be(user.Role);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_InvalidToken_ThrowsException()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => _jwtService.GetPrincipalFromExpiredToken(invalidToken));
    }
}