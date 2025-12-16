using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthSystem.Core.Entities;
using AuthSystem.Infrastructure.Services;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;

namespace AuthSystem.Application.UnitTests.Infrastructure;

public class JwtServiceIntegrationTests
{
    private readonly JwtService _jwtService;
    private readonly string _secretKey = "this-is-a-very-long-secret-key-for-testing-purposes-123456789";
    private readonly string _issuer = "TestIssuer";

    public JwtServiceIntegrationTests()
    {
        _jwtService = new JwtService(_secretKey, _issuer, 30, 7);
    }

    [Fact]
    public void TokenRoundTrip_GenerateAndValidate_ShouldWork()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Role = "Admin"
        };

        // Act
        var token = _jwtService.GenerateToken(user);
        var principal = _jwtService.GetPrincipalFromExpiredToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.FindFirst("nameid")?.Value.Should().Be(user.Id.ToString());
        principal.FindFirst("email")?.Value.Should().Be(user.Email);
        principal.FindFirst("role")?.Value.Should().Be(user.Role);
    }

    [Fact]
    public void GenerateToken_TokenExpiration_ShouldBeCorrect()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Role = "User"
        };
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _jwtService.GenerateToken(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        jwtToken.ValidTo.Should().BeAfter(beforeGeneration.AddMinutes(29));
        jwtToken.ValidTo.Should().BeBefore(beforeGeneration.AddMinutes(31));
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WithWrongSecretKey_ShouldThrow()
    {
        // Arrange
        var wrongKeyService = new JwtService("wrong-secret-key-123456789", _issuer, 30, 7);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Role = "User"
        };
        var token = _jwtService.GenerateToken(user);

        // Act & Assert
        Assert.ThrowsAny<SecurityTokenException>(() => wrongKeyService.GetPrincipalFromExpiredToken(token));
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WithWrongIssuer_ShouldThrow()
    {
        // Arrange
        var wrongIssuerService = new JwtService(_secretKey, "WrongIssuer", 30, 7);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Role = "User"
        };
        var token = _jwtService.GenerateToken(user);

        // Act & Assert
        Assert.ThrowsAny<SecurityTokenException>(() => wrongIssuerService.GetPrincipalFromExpiredToken(token));
    }

    [Fact]
    public void GenerateRefreshToken_MultipleGenerations_ShouldBeUnique()
    {
        // Arrange
        var tokens = new HashSet<string>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            tokens.Add(_jwtService.GenerateRefreshToken());
        }

        // Assert
        tokens.Should().HaveCount(100); // All tokens should be unique
    }

    [Fact]
    public void GenerateToken_DifferentUsers_ShouldHaveDifferentClaims()
    {
        // Arrange
        var user1 = new User { Id = Guid.NewGuid(), Email = "user1@example.com", Role = "User" };
        var user2 = new User { Id = Guid.NewGuid(), Email = "user2@example.com", Role = "Admin" };

        // Act
        var token1 = _jwtService.GenerateToken(user1);
        var token2 = _jwtService.GenerateToken(user2);

        // Assert
        var principal1 = _jwtService.GetPrincipalFromExpiredToken(token1);
        var principal2 = _jwtService.GetPrincipalFromExpiredToken(token2);

        principal1!.FindFirst("nameid")?.Value.Should().Be(user1.Id.ToString());
        principal2!.FindFirst("nameid")?.Value.Should().Be(user2.Id.ToString());
        
        principal1.FindFirst("email")?.Value.Should().Be(user1.Email);
        principal2.FindFirst("email")?.Value.Should().Be(user2.Email);
        
        principal1.FindFirst("role")?.Value.Should().Be("User");
        principal2.FindFirst("role")?.Value.Should().Be("Admin");
    }
}