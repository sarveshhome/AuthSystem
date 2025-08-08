using Microsoft.AspNetCore.Identity;

namespace AuthSystem.Core.Entities;

public class User: IdentityUser
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string Role { get; set; } = "User";
}