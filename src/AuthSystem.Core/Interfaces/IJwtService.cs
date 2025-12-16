using System.Security.Claims;
using AuthSystem.Core.Entities;

namespace AuthSystem.Core.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}