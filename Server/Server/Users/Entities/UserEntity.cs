using Server.Models;
using Server.Shared;

namespace Server.Users.Entities;

public class UserEntity:BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}

